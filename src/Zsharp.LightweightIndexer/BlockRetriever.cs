namespace Zsharp.LightweightIndexer
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NetMQ;
    using NetMQ.Sockets;
    using Zsharp.Rpc.Client;

    public sealed class BlockRetriever : IBlockRetriever
    {
        public const string SubscriptionTopic = "hashblock";

        readonly IRpcClientFactory rpc;
        readonly string publisherAddress;
        SubscriberSocket? subscriber;
        NetMQPoller? poller;
        CancellationTokenSource? canceler;
        Task? retriever;
        SemaphoreSlim? notification;
        bool disposed;

        public BlockRetriever(IRpcClientFactory rpc, string publisherAddress)
        {
            this.rpc = rpc;
            this.publisherAddress = publisherAddress;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsyncCore();
            this.Dispose(false);
        }

        public Task<Task> StartAsync(IBlockListener listener, CancellationToken cancellationToken = default)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (this.retriever != null)
            {
                throw new InvalidOperationException("The retriever is already started.");
            }

            // Subscribe to ZeroMQ.
            this.subscriber = new SubscriberSocket();

            try
            {
                this.poller = new NetMQPoller();

                try
                {
                    this.subscriber.ReceiveReady += (sender, e) => this.NotifyNewBlock();
                    this.subscriber.Connect(this.publisherAddress);
                    this.subscriber.Subscribe(SubscriptionTopic);

                    this.poller.Add(this.subscriber);
                    this.poller.RunAsync();

                    // Start background tasks to retrieve blocks.
                    this.canceler = new CancellationTokenSource();

                    try
                    {
                        this.retriever = this.RetrieveBlocksAsync(listener, this.canceler.Token);
                    }
                    catch
                    {
                        this.canceler.Dispose();
                        this.canceler = null;
                        throw;
                    }
                }
                catch
                {
                    this.poller.Dispose();
                    this.poller = null;
                    throw;
                }
            }
            catch
            {
                this.subscriber.Dispose();
                this.subscriber = null;
                throw;
            }

            return Task.FromResult(this.retriever);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            if (this.retriever == null)
            {
                throw new InvalidOperationException("The retriever is not running.");
            }

            Debug.Assert(this.canceler != null, $"{nameof(this.canceler)} is null.");
            Debug.Assert(this.subscriber != null, $"{nameof(this.subscriber)} is null.");
            Debug.Assert(this.poller != null, $"{nameof(this.poller)} is null.");

            // Trigger cancel.
            this.canceler.Cancel();

            // Wait until background tasks is completed.
            // We need to use Task.WhenAny() so it will not throw if there is an exception in the retriever.
            await Task.WhenAny(this.retriever);

            // Stop notification listenning.
            this.subscriber.Unsubscribe(SubscriptionTopic);
            this.subscriber.Disconnect(this.publisherAddress);

            this.poller.Stop();

            // Reset state.
            this.retriever = null;

            this.canceler.Dispose();
            this.canceler = null;

            this.subscriber.Dispose();
            this.subscriber = null;

            this.poller.Dispose();
            this.poller = null;
        }

        async Task RetrieveBlocksAsync(IBlockListener listener, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var height = await listener.GetStartBlockAsync(cancellationToken);

            while (true)
            {
                Block? block;

                await using (var client = await this.rpc.CreateChainInformationClientAsync(cancellationToken))
                {
                    block = await client.GetBlockAsync(height, cancellationToken);

                    if (block == null)
                    {
                        var info = await client.GetChainInfoAsync(cancellationToken);
                        var available = Convert.ToInt32(info.Blocks);

                        if (available >= height)
                        {
                            // A new block just comming.
                            continue;
                        }

                        if (available == height - 1)
                        {
                            // The target block is not available right now.
                            await this.WaitForNewBlockAsync(cancellationToken);
                            continue;
                        }

                        // There is a re-org happened.
                        await listener.DiscardBlocksAsync(available, cancellationToken);
                        height = available;
                        continue;
                    }
                }

                height = await listener.ProcessBlockAsync(block, height, cancellationToken);
            }
        }

        async Task WaitForNewBlockAsync(CancellationToken cancellationToken)
        {
            this.notification = new SemaphoreSlim(0);

            try
            {
                await this.notification.WaitAsync(cancellationToken);
            }
            finally
            {
                // We need to synchronize Dispose() and Release() due to it not thread safe.
                lock (this.notification)
                {
                    this.notification.Dispose();
                }

                this.notification = null;
            }
        }

        void NotifyNewBlock()
        {
            // Read received message to remove it from buffer.
            this.subscriber.ReceiveFrameString(); // "hashblock"
            this.subscriber.ReceiveFrameBytes(); // Block hash.
            this.subscriber.ReceiveFrameBytes(); // Sequence.

            // Raise event. We need to catch the object to prevent race condition.
            var trigger = this.notification;

            if (trigger == null)
            {
                // No one is waiting for notification.
                return;
            }

            // We need to synchronize Dispose() and Release() due to it not thread safe.
            lock (trigger)
            {
                try
                {
                    trigger.Release();
                }
                catch (ObjectDisposedException)
                {
                    // Ignore.
                }
            }
        }

        void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.retriever != null)
                {
                    this.StopAsync().Wait();
                }
            }

            this.disposed = true;
        }

        async ValueTask DisposeAsyncCore()
        {
            if (this.retriever != null)
            {
                await this.StopAsync();
            }
        }
    }
}
