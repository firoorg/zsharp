namespace Zsharp.Rpc.Client.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.Tests;
    using Xunit;

    public sealed class TransactionManagementClientTests : RpcClientTesting<TransactionManagementClient>
    {
        static readonly NodeConfigParameters Config = new NodeConfigParameters()
        {
            { "autocommit", "0" }
        };

        public TransactionManagementClientTests()
            : base(Config)
        {
        }

        [Fact]
        public async Task PublishAsync_WithValidTransaction_ShouldPublishSuccessfullyToNetwork()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var tx = Transaction.Parse(await issuer.IssueManagedAsync(owner), this.Client.Network);

            // Act.
            var hash = await this.Subject.PublishAsync(tx);
            var mined = this.Node.Generate(1).Single();

            // Assert.
            Assert.Equal(tx.GetHash(), hash);

            await using (var client = await this.Factory.CreateChainInformationClientAsync())
            {
                var block = await client.GetBlockAsync(mined);

                Assert.NotNull(block);
                Assert.Contains(block!.Transactions, t => t.GetHash() == hash);
            }
        }

        protected override TransactionManagementClient CreateSubject()
        {
            return new TransactionManagementClient(this.Factory, this.Client);
        }
    }
}
