namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Buffers;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;
    using Xunit;
    using Zsharp.Elysium;

    public sealed class ElysiumInformationClientTests : RpcClientTesting<ElysiumInformationClient>
    {
        [Fact]
        public async Task GetBalanceAsync_WithValidAddressAndProperty_ShouldReturnExpectedData()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            await issuer.IssueManagedAsync(owner);
            this.Node.Generate(1);

            var property = await this.GetPropertyAsync(issuer.Name);

            await this.GrantTokensAsync(property, owner, owner, new TokenAmount(100));
            this.Node.Generate(1);

            // Act.
            var result = await this.Subject.GetBalanceAsync(owner, property);

            // Assert.
            Assert.Equal(100, result.Balance.Value);
            Assert.Equal(0, result.Reserved.Value);
        }

        [Fact]
        public async Task GetGrantsAsync_WithValidProperty_ShouldReturnCorrectData()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var receiver = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var create = await issuer.IssueManagedAsync(owner);
            this.Node.Generate(1);

            var property = await this.GetPropertyAsync(issuer.Name);

            var grant = await this.GrantTokensAsync(property, owner, receiver, new TokenAmount(1));
            this.Node.Generate(1);

            // Act.
            var result = await this.Subject.GetGrantsAsync(property);

            // Assert.
            var history = Assert.Single(result.GrantHistories);

            Assert.Equal(uint256.Parse(create), result.PropertyCreationTransaction);
            Assert.Equal(property.Id, result.PropertyId);
            Assert.Equal(owner, result.PropertyIssuer);
            Assert.Equal(issuer.Name, result.PropertyName);
            Assert.Equal(1, result.TotalTokens.Value);

            Assert.Equal(1, history.Amount.Value);
            Assert.Equal(uint256.Parse(grant), history.Transaction);
            Assert.Equal(TokenGrantType.Grant, history.Type);
        }

        [Fact]
        public async Task GetPayloadAsync_WithInvalidTransaction_ShouldThrow()
        {
            await Assert.ThrowsAsync<RPCException>(() => this.Subject.GetPayloadAsync(uint256.One));
        }

        [Fact]
        public async Task GetPayloadAsync_WithNonElysiumTransaction_ShouldReturnNull()
        {
            // Arrange.
            var block = this.Node.Generate(1).Single();
            var tx = (await this.GetTransactionsAsync(block)).Single();

            // Act.
            var result = await this.Subject.GetPayloadAsync(tx.GetHash());

            // Assert.
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPayloadAsync_WithElysiumTransaction_ShouldReturnItPayload()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            issuer.Type = PropertyType.Production;
            issuer.TokenType = TokenType.Indivisible;
            issuer.Current = null;
            issuer.Category = "Company";
            issuer.Subcategory = "Private";
            issuer.Name = "Satang Corporation";
            issuer.Url = "https://satang.com";
            issuer.Description = "Provides cryptocurrency solutions.";

            var tx = await issuer.IssueManagedAsync(owner);
            this.Node.Generate(1);

            // Act.
            var result = await this.Subject.GetPayloadAsync(uint256.Parse(tx));

            // Assert.
            Assert.NotNull(result);
            Assert.Equal(
                "0000003601000100000000436F6D70616E79005072697661746500536174616E6720436F72706F726174696F6E0068747470733A2F2F736174616E672E636F6D0050726F76696465732063727970746F63757272656E637920736F6C7574696F6E732E00",
                BitConverter.ToString(result.Value.ToArray()).Replace("-", ""));
        }

        [Fact]
        public async Task GetTransactionAsync_WithInvalidTransaction_ShouldThrow()
        {
            await Assert.ThrowsAsync<RPCException>(() => this.Subject.GetTransactionAsync(uint256.One));
        }

        [Fact]
        public async Task GetTransactionAsync_WithNonElysiumTransaction_ShouldReturnNull()
        {
            // Arrange.
            var block = this.Node.Generate(1).Single();
            var tx = (await this.GetTransactionsAsync(block)).Single();

            // Act.
            var result = await this.Subject.GetTransactionAsync(tx.GetHash());

            // Assert.
            Assert.Null(result);
        }

        [Fact]
        public async Task GetTransactionAsync_WithElysiumTransaction_ShouldReturnCorrectData()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var tx = uint256.Parse(await issuer.IssueManagedAsync(owner));
            var block = this.Node.Generate(1).Single();

            // Act.
            var result = await this.Subject.GetTransactionAsync(tx);

            // Assert.
            Assert.NotNull(result);
            Assert.NotEqual(Money.Zero, result!.Fee);
            Assert.True(result.Owned);
            Assert.Null(result.ReferenceAddress);
            Assert.Equal(owner, result.SendingAddress);
            Assert.Equal(tx, result.Id);
            Assert.Equal("Create Property - Manual", result.Name);
            Assert.Equal(54, result.Type);
            Assert.Equal(0, result.Version);

            var confirmation = result.Confirmation;

            Assert.NotNull(confirmation);
            Assert.Equal(103, confirmation!.Block);
            Assert.Equal(block, confirmation.BlockHash);
            Assert.Equal(1, confirmation.BlockIndex);
            Assert.NotEqual(default(DateTime), confirmation.BlockTime);
            Assert.Equal(1, confirmation.Count);
            Assert.Null(confirmation.InvalidReason);
            Assert.True(confirmation.Valid);
        }

        [Fact]
        public async Task ListPropertiesAsync_WhenInvoke_ShouldReturnCorrectData()
        {
            // Act.
            var result = await this.Subject.ListPropertiesAsync();

            // Assert.
            Assert.Equal(2, result.Count());

            Assert.Equal("N/A", result.First().Category);
            Assert.Equal("Exodus serve as the binding between Zcoin, smart properties and contracts created on the Exodus Layer.", result.First().Description);
            Assert.Equal(new PropertyId(1), result.First().Id);
            Assert.Equal("Exodus", result.First().Name);
            Assert.Equal("N/A", result.First().Subcategory);
            Assert.Equal(TokenType.Divisible, result.First().Type);
            Assert.Equal("https://www.zcoin.io", result.First().WebsiteUrl);

            Assert.Equal("N/A", result.Last().Category);
            Assert.Equal("Test Exodus serve as the binding between Zcoin, smart properties and contracts created on the Exodus Layer.", result.Last().Description);
            Assert.Equal(new PropertyId(2), result.Last().Id);
            Assert.Equal("Test Exodus", result.Last().Name);
            Assert.Equal("N/A", result.Last().Subcategory);
            Assert.Equal(TokenType.Divisible, result.Last().Type);
            Assert.Equal("https://www.zcoin.io", result.Last().WebsiteUrl);
        }

        protected override ElysiumInformationClient CreateSubject()
        {
            return new ElysiumInformationClient(this.Factory, this.Node.CreateRPCClient());
        }
    }
}
