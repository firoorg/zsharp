namespace Zsharp.Rpc.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;
    using Zsharp.Elysium;

    public sealed class RpcClientFactory : IRpcClientFactory
    {
        readonly Network network;

        public RpcClientFactory(
            Network network,
            Uri server,
            RPCCredentialString credential,
            ITransactionSerializer elysiumSerializer)
        {
            this.network = network;
            this.ServerUrl = server;
            this.Credential = credential;
            this.ElysiumSerializer = elysiumSerializer;
            this.GenesisTransactions = new HashSet<uint256>(network.GetGenesis().Transactions.Select(t => t.GetHash()));
        }

        public RPCCredentialString Credential { get; }

        public ITransactionSerializer ElysiumSerializer { get; }

        public ISet<uint256> GenesisTransactions { get; }

        public Uri ServerUrl { get; }

        public async ValueTask<IChainInformationClient> CreateChainInformationClientAsync(
            CancellationToken cancellationToken = default)
        {
            return new ChainInformationClient(this, await this.CreateClientAsync(cancellationToken));
        }

        public async ValueTask<IElysiumInformationClient> CreateElysiumInformationClientAsync(
            CancellationToken cancellationToken = default)
        {
            return new ElysiumInformationClient(this, await this.CreateClientAsync(cancellationToken));
        }

        public async ValueTask<IPropertyManagementClient> CreatePropertyManagementClientAsync(
            CancellationToken cancellationToken = default)
        {
            return new PropertyManagementClient(this, await this.CreateClientAsync(cancellationToken));
        }

        public async ValueTask<ITransactionManagementClient> CreateTransactionManagementClientAsync(
            CancellationToken cancellationToken = default)
        {
            return new TransactionManagementClient(this, await this.CreateClientAsync(cancellationToken));
        }

        public async ValueTask<IWalletClient> CreateWalletClientAsync(CancellationToken cancellationToken = default)
        {
            return new WalletClient(this, await this.CreateClientAsync(cancellationToken));
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }

        async Task<RPCClient> CreateClientAsync(CancellationToken cancellationToken = default)
        {
            var client = new RPCClient(this.Credential, this.ServerUrl, this.network);

            await client.ScanRPCCapabilitiesAsync();

            return client;
        }
    }
}
