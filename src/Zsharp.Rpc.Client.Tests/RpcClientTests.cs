namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using NBitcoin;
    using NBitcoin.RPC;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium.Transactions;

    public sealed class RpcClientTests : RpcClientTesting<FakeRpcClient>
    {
        [Fact]
        public void Constructor_WhenSuccess_ShouldInitializeProperties()
        {
            Assert.Same(Client, this.Subject.Client);
            Assert.Same(Factory, this.Subject.Factory);
        }

        [Fact]
        public void Dispose_WhenInvoke_ShouldInvokeProtectedOverload()
        {
            this.Subject.Dispose();

            this.Subject.StubbedDispose.Verify(f => f(true), Times.Once());
            this.Subject.StubbedDisposeAsyncCore.Verify(f => f(), Times.Never());
        }

        [Fact]
        public async Task DisposeAsync_WhenInvoke_ShouldInvokeDisposeAsyncCore()
        {
            await this.Subject.DisposeAsync();

            this.Subject.StubbedDisposeAsyncCore.Verify(f => f(), Times.Once());
            this.Subject.StubbedDispose.Verify(f => f(false), Times.Once());
        }

        [Fact]
        public async Task PopulateElysiumInformationAsync_WithGenesisTransaction_ShouldNotPopulate()
        {
            foreach (var tx in this.Network.GetGenesis().Transactions)
            {
                await this.Subject.PopulateElysiumInformationAsync(tx);

                Assert.Null(tx.GetElysiumTransaction());
            }
        }

        [Fact]
        public async Task PopulateElysiumInformationAsync_WithInvalidTransaction_ShouldThrow()
        {
            var tx = Transaction.Create(this.Network);

            await Assert.ThrowsAsync<RPCException>(() => this.Subject.PopulateElysiumInformationAsync(tx));
        }

        [Fact]
        public async Task PopulateElysiumInformationAsync_WithNonElysiumTransaction_ShouldNotPopulate()
        {
            // Arrange.
            var receiver = await this.GenerateNewAddressAsync();

            this.Node.Generate(101);
            await this.FundAddressAsync(receiver, Money.Coins(10));

            var hash = this.Node.Generate(1).Single();
            var block = await this.GetBlockAsync(hash);

            var coinbase = block.Transactions[0];
            var transfer = block.Transactions[1];

            // Act.
            await this.Subject.PopulateElysiumInformationAsync(coinbase);
            await this.Subject.PopulateElysiumInformationAsync(transfer);

            // Assert.
            Assert.Null(coinbase.GetElysiumTransaction());
            Assert.Null(transfer.GetElysiumTransaction());
        }

        [Fact]
        public async Task PopulateElysiumInformationAsync_WithUnconfirmedTransaction_ShouldThrow()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var receiver = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var hash = uint256.Parse(await issuer.IssueManagedAsync(owner));
            Transaction? tx;

            await using (var client = await this.Factory.CreateChainInformationClientAsync())
            {
                tx = await client.GetTransactionAsync(hash);
            }

            Assert.NotNull(tx);

            // Act.
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                "tx",
                () => Subject.PopulateElysiumInformationAsync(tx!));

            // Assert.
            Assert.Equal("The transaction is not confirmed. (Parameter 'tx')", ex.Message);
        }

        [Fact]
        public async Task PopulateElysiumInformationAsync_WithValidElysiumTransaction_ShouldPopulate()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var hash = uint256.Parse(await issuer.IssueManagedAsync(owner));
            this.Node.Generate(1);

            Transaction? tx;

            await using (var client = await this.Factory.CreateChainInformationClientAsync())
            {
                tx = await client.GetTransactionAsync(hash);
            }

            Assert.NotNull(tx);

            // Act.
            await this.Subject.PopulateElysiumInformationAsync(tx!);

            // Assert.
            var elysium = Assert.IsType<CreateManagedPropertyV0>(tx!.GetElysiumTransaction());

            Assert.NotNull(elysium);
            Assert.Equal(issuer.Category, elysium.Category);
            Assert.Equal(issuer.Description, elysium.Description);
            Assert.Equal(CreateManagedPropertyV0.StaticId, elysium.Id);
            Assert.Equal(issuer.Name, elysium.Name);
            Assert.Null(elysium.PreviousId);
            Assert.Null(elysium.Receiver);
            Assert.Equal(owner, elysium.Sender);
            Assert.Equal(issuer.Subcategory, elysium.Subcategory);
            Assert.Equal(issuer.TokenType, elysium.TokenType);
            Assert.Equal(issuer.Type, elysium.Type);
            Assert.Equal(0, elysium.Version);
            Assert.Equal(issuer.Url, elysium.Website);
        }

        protected override FakeRpcClient CreateSubject()
        {
            return new FakeRpcClient(Factory, Client);
        }
    }
}
