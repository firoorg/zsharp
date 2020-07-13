namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using NBitcoin;
    using NBitcoin.RPC;
    using NBitcoin.Tests;
    using Xunit;
    using Zsharp.Elysium;
    using Zsharp.Testing;

    public sealed class RpcClientFactoryTests : IDisposable
    {
        readonly NodeBuilder nodeBuilder;
        readonly Network network;
        readonly Uri server;
        readonly RPCCredentialString credential;
        readonly Mock<ITransactionSerializer> elysiumSerializer;
        readonly RpcClientFactory subject;

        public RpcClientFactoryTests()
        {
            this.nodeBuilder = NodeBuilderFactory.CreateNodeBuilder(GetType());

            try
            {
                var node = this.nodeBuilder.CreateNode(true);

                this.network = node.Network;
                this.server = node.RPCUri;
                this.credential = RPCCredentialString.Parse(node.GetRPCAuth());
                this.elysiumSerializer = new Mock<ITransactionSerializer>();
                this.subject = new RpcClientFactory(this.network, this.server, this.credential, this.elysiumSerializer.Object);
            }
            catch
            {
                this.nodeBuilder.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            this.nodeBuilder.Dispose();
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            var genesis = this.network.GetGenesis().Transactions
                .Select(t => t.GetHash())
                .ToHashSet();

            Assert.Same(this.elysiumSerializer.Object, this.subject.ElysiumSerializer);
            Assert.Equal(genesis.Count, this.subject.GenesisTransactions.Count);
            Assert.Subset(genesis, this.subject.GenesisTransactions);
        }

        [Fact]
        public async Task CreateChainInformationClientAsync_WhenInvoke_ShouldSuccess()
        {
            var result = await this.subject.CreateChainInformationClientAsync();

            await result.DisposeAsync();
        }

        [Fact]
        public async Task CreateElysiumInformationClientAsync_WhenInvoke_ShouldSuccess()
        {
            var result = await this.subject.CreateElysiumInformationClientAsync();

            await result.DisposeAsync();
        }

        [Fact]
        public async Task CreatePropertyManagementClientAsync_WhenInvoke_ShouldSuccess()
        {
            var result = await this.subject.CreatePropertyManagementClientAsync();

            await result.DisposeAsync();
        }

        [Fact]
        public async Task CreateTransactionManagementClientAsync_WhenInvoke_ShouldSuccess()
        {
            var result = await this.subject.CreateTransactionManagementClientAsync();

            await result.DisposeAsync();
        }

        [Fact]
        public async Task CreateWalletClientAsync_WhenInvoke_ShouldSuccess()
        {
            var result = await this.subject.CreateWalletClientAsync();

            await result.DisposeAsync();
        }
    }
}
