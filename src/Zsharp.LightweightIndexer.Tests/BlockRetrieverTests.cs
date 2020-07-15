namespace Zsharp.LightweightIndexer.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NBitcoin;
    using NBitcoin.RPC;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Zsharp.Rpc.Client;
    using Zsharp.Testing;
    using static Zsharp.Testing.AsynchronousTesting;

    public sealed class BlockRetrieverTests : IDisposable
    {
        readonly PublisherSocket publisher;
        readonly Mock<IChainInformationClient> chainInformationClient;
        readonly Mock<IRpcClientFactory> rpc;
        readonly Mock<IBlockListener> listener;
        readonly BlockRetriever subject;

        public BlockRetrieverTests()
        {
            this.publisher = new PublisherSocket();

            try
            {
                var publisherPort = this.publisher.BindRandomPort("tcp://localhost");

                this.chainInformationClient = new Mock<IChainInformationClient>();

                this.rpc = new Mock<IRpcClientFactory>();
                this.rpc
                    .Setup(f => f.CreateChainInformationClientAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(this.chainInformationClient.Object);

                this.listener = new Mock<IBlockListener>();
                this.subject = new BlockRetriever(this.rpc.Object, "tcp://localhost:" + publisherPort);
            }
            catch
            {
                this.publisher.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            this.subject.Dispose();
            this.publisher.Dispose();
        }

        [Fact]
        public void Dispose_NotStarted_ShouldNotThrow()
        {
            this.subject.Dispose();
        }

        [Fact]
        public async Task Dispose_AlreadyStarted_ShouldStop()
        {
            // Arrange.
            this.MakeRetrieverEndlessLoop();

            var retriever = await this.subject.StartAsync(this.listener.Object);

            // Act.
            this.subject.Dispose();

            // Assert.
            Assert.True(retriever.IsCanceled);
        }

        [Fact]
        public async Task Dispose_AlreadyStopped_ShouldNotThrow()
        {
            // Arrange.
            this.MakeRetrieverEndlessLoop();

            await this.subject.StartAsync(this.listener.Object);
            await this.subject.StopAsync();

            // Act.
            this.subject.Dispose();
        }

        [Fact]
        public async Task DisposeAsync_NotStarted_ShouldNotThrow()
        {
            await this.subject.DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsync_AlreadyStarted_ShouldStop()
        {
            // Arrange.
            this.MakeRetrieverEndlessLoop();

            var retriever = await this.subject.StartAsync(this.listener.Object);

            // Act.
            await this.subject.DisposeAsync();

            // Assert.
            Assert.True(retriever.IsCanceled);
        }

        [Fact]
        public async Task DisposeAsync_AlreadyStopped_ShouldNotThrow()
        {
            // Arrange.
            this.MakeRetrieverEndlessLoop();

            await this.subject.StartAsync(this.listener.Object);
            await this.subject.StopAsync();

            // Act.
            await this.subject.DisposeAsync();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task StartAsync_AlreadyDisposed_ShouldThrow(bool async)
        {
            if (async)
            {
                await this.subject.DisposeAsync();
            }
            else
            {
                this.subject.Dispose();
            }

            var ex = await Assert.ThrowsAsync<ObjectDisposedException>(
                () => this.subject.StartAsync(this.listener.Object));

            Assert.Equal(this.subject.GetType().FullName, ex.ObjectName);
        }

        [Fact]
        public async Task StartAsync_AlreadyStarted_ShouldThrow()
        {
            // Arrange.
            this.MakeRetrieverEndlessLoop();

            await this.subject.StartAsync(this.listener.Object, CancellationToken.None);

            // Act.
            await Assert.ThrowsAsync<InvalidOperationException>(() => this.subject.StartAsync(this.listener.Object));
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncReturnIncomingBlock_ShouldRetryImmediately()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block = TestBlock.Regtest1;
                var processingStarted = new TaskCompletionSource<bool>();

                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                this.chainInformationClient
                    .SetupSequence(c => c.GetBlockAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Block?)null)
                    .ReturnsAsync(block);

                this.chainInformationClient
                    .Setup(c => c.GetChainInfoAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new BlockchainInfo() { Blocks = 1 });

                this.listener
                    .Setup(l => l.ProcessBlockAsync(
                        It.IsAny<Block>(),
                        It.IsAny<int>(),
                        It.IsNotIn(CancellationToken.None)))
                    .Returns((Block block, int height, CancellationToken cancellationToken) =>
                    {
                        var canceled = new TaskCompletionSource<int>();

                        processingStarted.SetResult(true);
                        cancellationToken.Register(canceled.SetCanceled);

                        return canceled.Task;
                    });

                // Act.
                await this.subject.StartAsync(this.listener.Object, cancellationToken);
                await processingStarted.Task;

                // Assert.
                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Exactly(2));

                this.chainInformationClient.Verify(
                    c => c.GetChainInfoAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.ProcessBlockAsync(block, 1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncReturnNonExistBlock_ShouldWaitForNewBlockNotification()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                this.chainInformationClient
                    .SetupSequence(c => c.GetBlockAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Block?)null);

                this.chainInformationClient
                    .Setup(c => c.GetChainInfoAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new BlockchainInfo() { Blocks = 0 });

                // Act.
                var retriever = await this.subject.StartAsync(this.listener.Object, cancellationToken);

                await this.subject.StopAsync();

                // Assert.
                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetChainInfoAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                Assert.True(retriever.IsCanceled);
            });
        }

        [Fact]
        public Task StartAsync_WhenWaitingForNewBlockNotification_ShouldRetryWhenReceivedNotification()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block = TestBlock.Regtest1;
                var waitStarted = new TaskCompletionSource<int>();
                var wakedUp = new TaskCompletionSource<int>();

                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                this.chainInformationClient
                    .SetupSequence(c => c.GetBlockAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Block?)null)
                    .ReturnsAsync(() =>
                    {
                        wakedUp.SetResult(0);
                        return block;
                    });

                this.chainInformationClient
                    .Setup(c => c.GetChainInfoAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => waitStarted.SetResult(0))
                    .ReturnsAsync(new BlockchainInfo() { Blocks = 0 });

                this.listener
                    .Setup(l => l.ProcessBlockAsync(block, 1, It.IsNotIn(CancellationToken.None)))
                    .Returns((Block block, int height, CancellationToken cancellationToken) =>
                    {
                        var canceled = new TaskCompletionSource<int>();
                        cancellationToken.Register(canceled.SetCanceled);
                        return canceled.Task;
                    });

                await this.subject.StartAsync(this.listener.Object, cancellationToken);
                await waitStarted.Task;

                // Act.
                this.publisher
                    .SendMoreFrame(BlockRetriever.SubscriptionTopic)
                    .SendMoreFrame(uint256.One.ToBytes(false))
                    .SendFrame(BitConverter.GetBytes(0));

                await wakedUp.Task;
                await this.subject.StopAsync();

                // Assert.
                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Exactly(2));

                this.chainInformationClient.Verify(
                    c => c.GetChainInfoAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.ProcessBlockAsync(block, 1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncReturnRemovedBlock_ShouldDiscardAllRemovedBlocks()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block = TestBlock.Regtest1;
                var processingStarted = new TaskCompletionSource<int>();

                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(3);

                this.chainInformationClient
                    .Setup(c => c.GetBlockAsync(3, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Block?)null);

                this.chainInformationClient
                    .Setup(c => c.GetChainInfoAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new BlockchainInfo() { Blocks = 1 });

                this.chainInformationClient
                    .Setup(c => c.GetBlockAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(block);

                this.listener
                    .Setup(l => l.ProcessBlockAsync(block, 1, It.IsNotIn(CancellationToken.None)))
                    .Returns((Block block, int height, CancellationToken cancellationToken) =>
                    {
                        var canceled = new TaskCompletionSource<int>();

                        cancellationToken.Register(canceled.SetCanceled);
                        processingStarted.SetResult(0);

                        return canceled.Task;
                    });

                // Act.
                await this.subject.StartAsync(this.listener.Object, cancellationToken);
                await processingStarted.Task;

                // Assert.
                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(3, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetChainInfoAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.DiscardBlocksAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.ProcessBlockAsync(block, 1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncReturnReturnExistingBlock_ShouldRetrieveAndProcessThatBlock()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block0 = TestBlock.Regtest0;
                var block1 = TestBlock.Regtest1;
                var finished = new TaskCompletionSource<int>();

                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                this.chainInformationClient
                    .Setup(c => c.GetBlockAsync(0, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(block0);

                this.listener
                    .Setup(l => l.ProcessBlockAsync(block0, 0, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                this.chainInformationClient
                    .Setup(c => c.GetBlockAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(block1);

                this.listener
                    .Setup(l => l.ProcessBlockAsync(block1, 1, It.IsAny<CancellationToken>()))
                    .Returns((Block block, int height, CancellationToken cancellationToken) =>
                    {
                        var canceled = new TaskCompletionSource<int>();

                        cancellationToken.Register(canceled.SetCanceled);
                        finished.SetResult(0);

                        return canceled.Task;
                    });

                // Act.
                await this.subject.StartAsync(this.listener.Object, cancellationToken);
                await finished.Task;

                // Assert.
                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(0, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.ProcessBlockAsync(block0, 0, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.chainInformationClient.Verify(
                    c => c.GetBlockAsync(1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.listener.Verify(
                    l => l.ProcessBlockAsync(block1, 1, It.IsNotIn(cancellationToken, default)),
                    Times.Once());
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task StopAsync_AlreadyDisposed_ShouldThrow(bool async)
        {
            if (async)
            {
                await this.subject.DisposeAsync();
            }
            else
            {
                this.subject.Dispose();
            }

            var ex = await Assert.ThrowsAsync<ObjectDisposedException>(() => this.subject.StopAsync());

            Assert.Equal(this.subject.GetType().FullName, ex.ObjectName);
        }

        [Fact]
        public async Task StopAsync_WhenNotRunning_ShouldThrow()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => this.subject.StopAsync());
        }

        [Fact]
        public Task StopAsync_RetrieverError_ShouldNotThrow() => WithCancellationTokenAsync(async cancellationToken =>
        {
            // Arrange.
            this.listener
                .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            var retriever = await this.subject.StartAsync(this.listener.Object, cancellationToken);

            // Act.
            await this.subject.StopAsync();

            // Assert.
            Assert.True(retriever.IsFaulted);

            this.listener.Verify(
                l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                Times.Once());
        });

        [Fact]
        public Task StopAsync_RetrieverCancelled_ShouldNotThrow()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.listener
                    .Setup(l => l.GetStartBlockAsync(It.IsAny<CancellationToken>()))
                    .Returns((CancellationToken cancellationToken) =>
                    {
                        var canceled = new TaskCompletionSource<int>();
                        cancellationToken.Register(canceled.SetCanceled);
                        return canceled.Task;
                    });

                var retriever = await this.subject.StartAsync(this.listener.Object, cancellationToken);

                // Act.
                await this.subject.StopAsync();

                // Assert.
                Assert.True(retriever.IsCanceled);

                this.listener.Verify(
                    l => l.GetStartBlockAsync(It.IsNotIn(cancellationToken, default)),
                    Times.Once());
            });
        }

        void MakeRetrieverEndlessLoop()
        {
            var block = TestBlock.Regtest0;

            this.listener
                .Setup(l => l.GetStartBlockAsync(It.IsNotIn(CancellationToken.None)))
                .ReturnsAsync((CancellationToken cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return 0;
                });

            this.chainInformationClient
                .Setup(c => c.GetBlockAsync(0, It.IsNotIn(CancellationToken.None)))
                .ReturnsAsync((int height, CancellationToken cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return block;
                });

            this.listener
                .Setup(l => l.ProcessBlockAsync(block, 0, It.IsNotIn(CancellationToken.None)))
                .ReturnsAsync((Block block, int height, CancellationToken cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return 0;
                });
        }
    }
}
