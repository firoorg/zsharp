namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using Xunit;
    using Zsharp.Elysium;
    using Zsharp.Testing;

    public sealed class PropertyManagementClientTests : RpcClientTesting<PropertyManagementClient>
    {
        readonly Property fakeProperty;

        public PropertyManagementClientTests()
        {
            this.fakeProperty = new Property(new PropertyId(3), "abc", "", "", "", "", TokenType.Indivisible);
        }

        [Fact]
        public async Task CreateManagedAsync_WithValidArgs_ShouldCreateNewManagedProperty()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            // Act.
            var tx = await Subject.CreateManagedAsync(
                owner,
                PropertyType.Production,
                TokenType.Indivisible,
                null,
                "Company",
                "Private",
                "Satang Corporation",
                "https://satang.com",
                "Provides cryptocurrency solutions.");

            this.Node.Generate(1);

            // Assert.
            await using (var client = await this.Factory.CreateElysiumInformationClientAsync())
            {
                var props = await client.ListPropertiesAsync();

                Assert.Equal(3, props.Count());
                Assert.Equal(3, props.Last().Id.Value);
                Assert.Equal("Satang Corporation", props.Last().Name);
                Assert.Equal("Company", props.Last().Category);
                Assert.Equal("Private", props.Last().Subcategory);
                Assert.Equal("https://satang.com", props.Last().WebsiteUrl);
                Assert.Equal("Provides cryptocurrency solutions.", props.Last().Description);
                Assert.Equal(TokenType.Indivisible, props.Last().Type);
            }
        }

        [Fact]
        public async Task GrantTokensAsync_WithInvalidAmount_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "amount",
                () => this.Subject.GrantTokensAsync(
                    this.fakeProperty,
                    TestAddress.Regtest1,
                    TestAddress.Regtest2,
                    TokenAmount.Zero,
                    null));
        }

        [Fact]
        public async Task GrantTokensAsync_WithValidArgs_ShouldGrantSuccess()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var receiver = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var create = uint256.Parse(await issuer.IssueManagedAsync(owner));
            this.Node.Generate(1);

            var property = await this.GetPropertyAsync(issuer.Name);

            // Act.
            var tx = await Subject.GrantTokensAsync(
                property,
                owner,
                receiver,
                new TokenAmount(100),
                "abc");

            this.Node.Generate(1);

            // Assert.
            await using (var client = await this.Factory.CreateElysiumInformationClientAsync())
            {
                var grant = await client.GetGrantsAsync(property);
                var history = Assert.Single(grant.GrantHistories);

                Assert.Equal(create, grant.PropertyCreationTransaction);
                Assert.Equal(property.Id, grant.PropertyId);
                Assert.Equal(owner, grant.PropertyIssuer);
                Assert.Equal(issuer.Name, grant.PropertyName);
                Assert.Equal(100, grant.TotalTokens.Value);

                Assert.Equal(100, history.Amount.Value);
                Assert.Equal(uint256.Parse(tx), history.Transaction);
                Assert.Equal(TokenGrantType.Grant, history.Type);
            }
        }

        [Fact]
        public async Task SendTokensAsync_WithInvalidAmount_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "amount",
                () => this.Subject.SendTokensAsync(
                    this.fakeProperty,
                    TestAddress.Regtest1,
                    TestAddress.Regtest2,
                    TokenAmount.Zero,
                    null));
        }

        [Fact]
        public async Task SendTokensAsync_WithInvalidReferenceAmount_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "referenceAmount",
                () => this.Subject.SendTokensAsync(
                    this.fakeProperty,
                    TestAddress.Regtest1,
                    TestAddress.Regtest2,
                    new TokenAmount(1),
                    Money.Zero));
        }

        [Fact]
        public async Task SendTokensAsync_WithNullReferenceAmount_ShouldAutomaticallyChooseOutputAmount()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var receiver = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            await issuer.IssueManagedAsync(owner);
            this.Node.Generate(1);

            var property = await this.GetPropertyAsync(issuer.Name);

            await this.GrantTokensAsync(property, owner, owner, new TokenAmount(100));
            this.Node.Generate(1);

            // Act.
            var result = await this.Subject.SendTokensAsync(
                property,
                owner,
                receiver,
                new TokenAmount(1),
                null);

            var block = this.Node.Generate(1).Single();

            // Assert.
            var tx = (await this.GetTransactionsAsync(block)).Single(t => t.GetHash() == uint256.Parse(result));
            var output = tx.Outputs.Single(o => o.ScriptPubKey.GetDestinationAddress(this.Network) == receiver);

            Assert.True(output.Value > Money.Zero);

            await using (var client = await this.Factory.CreateElysiumInformationClientAsync())
            {
                var balance = await client.GetBalanceAsync(receiver, property);

                Assert.Equal(1, balance.Balance.Value);
                Assert.Equal(0, balance.Reserved.Value);
            }
        }

        [Fact]
        public async Task SendTokensAsync_WithReferenceAmount_ShouldOutputByThatAmount()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var receiver = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(10));
            this.Node.Generate(1);

            await issuer.IssueManagedAsync(owner);
            this.Node.Generate(1);

            var property = await this.GetPropertyAsync(issuer.Name);

            await this.GrantTokensAsync(property, owner, owner, new TokenAmount(100));
            this.Node.Generate(1);

            // Act.
            var result = await Subject.SendTokensAsync(
                property,
                owner,
                receiver,
                new TokenAmount(1),
                Money.Coins(0.01m));

            var block = this.Node.Generate(1).Single();

            // Assert.
            var tx = (await this.GetTransactionsAsync(block)).Single(t => t.GetHash() == uint256.Parse(result));
            var output = tx.Outputs.Single(o => o.ScriptPubKey.GetDestinationAddress(this.Network) == receiver);

            Assert.Equal(Money.Coins(0.01m), output.Value);

            await using (var client = await this.Factory.CreateElysiumInformationClientAsync())
            {
                var balance = await client.GetBalanceAsync(receiver, property);

                Assert.Equal(1, balance.Balance.Value);
                Assert.Equal(0, balance.Reserved.Value);
            }
        }

        protected override PropertyManagementClient CreateSubject()
        {
            return new PropertyManagementClient(this.Factory, this.Node.CreateRPCClient());
        }
    }
}
