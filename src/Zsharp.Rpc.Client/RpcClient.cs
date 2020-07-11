namespace Zsharp.Rpc.Client
{
    using System;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;
    using Zsharp.Bitcoin;

    public abstract class RpcClient : IAsyncDisposable, IDisposable
    {
        protected RpcClient(RpcClientFactory factory, RPCClient client)
        {
            this.Factory = factory;
            this.Client = client;
        }

        protected RPCClient Client { get; }

        protected RpcClientFactory Factory { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsyncCore();
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            return new ValueTask(Task.CompletedTask);
        }

        protected async Task PopulateElysiumInformationAsync(
            Transaction tx,
            CancellationToken cancellationToken = default)
        {
            if (this.Factory.GenesisTransactions.Contains(tx.GetHash()))
            {
                return;
            }

            tx.SetElysiumTransaction(await this.TryDecodeElysiumTransactionAsync(tx, cancellationToken));
        }

        // This method does not support unconfirmed transaction.
        async Task<Elysium.Transaction?> TryDecodeElysiumTransactionAsync(
            Transaction tx,
            CancellationToken cancellationToken = default)
        {
            // Get Elysium's payload.
            ElysiumTransaction? info;
            ReadOnlySequence<byte>? payload;

            await using (var client = await this.Factory.CreateElysiumInformationClientAsync(cancellationToken))
            {
                info = await client.GetTransactionAsync(tx.GetHash(), cancellationToken);

                if (info == null)
                {
                    // Transaction found but it is not Elysium transaction.
                    return null;
                }

                // Check if transaction still in the mempool. If transaction is not in the block we don't know if it is
                // valid or not.
                if (info.Confirmation == null)
                {
                    throw new ArgumentException("The transaction is not confirmed.", nameof(tx));
                }

                if (!info.Confirmation.Valid)
                {
                    return null;
                }

                // The returned value will never be null due to the above statements already make sure it is Elysium
                // transaction.
                payload = await client.GetPayloadAsync(tx.GetHash(), cancellationToken);
            }

            return this.DeserializeElysiumTransaction(info.SendingAddress, info.ReferenceAddress, payload.Value);
        }

        Elysium.Transaction DeserializeElysiumTransaction(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            in ReadOnlySequence<byte> data)
        {
            var reader = new SequenceReader<byte>(data);

            return this.Factory.ElysiumSerializer.Deserialize(sender, receiver, ref reader);
        }
    }
}
