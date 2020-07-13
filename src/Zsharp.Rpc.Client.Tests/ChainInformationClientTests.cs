namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium.Transactions;

    public sealed class ChainInformationClientTests : RpcClientTesting<ChainInformationClient>
    {
        [Fact]
        public async Task GetBlockAsync_WithNonExistsHash_ShouldReturnNull()
        {
            var block = await this.Subject.GetBlockAsync(uint256.One);

            Assert.Null(block);
        }

        [Fact]
        public async Task GetBlockAsync_WithGenesisHash_ShouldReturnGenesisBlock()
        {
            var genesis = this.Network.GenesisHash;

            var block = await this.Subject.GetBlockAsync(genesis);

            Assert.NotNull(block);
            Assert.Equal(genesis, block!.GetHash());
            Assert.All(block.Transactions, tx => Assert.Null(tx.GetElysiumTransaction()));
        }

        [Fact]
        public Task GetBlockAsync_WithNegativeHeight_ShouldThrow()
        {
            return Assert.ThrowsAsync<ArgumentOutOfRangeException>("height", () => this.Subject.GetBlockAsync(-1));
        }

        [Fact]
        public async Task GetBlockAsync_WithInvalidHeight_ShouldReturnNull()
        {
            var block = await this.Subject.GetBlockAsync(1);

            Assert.Null(block);
        }

        [Fact]
        public async Task GetBlockAsync_WithGenesisHeight_ShouldReturnGenesisBlock()
        {
            var block = await this.Subject.GetBlockAsync(0);

            Assert.NotNull(block);
            Assert.Equal(this.Network.GenesisHash, block!.GetHash());
        }

        [Fact]
        public async Task GetBlockAsync_WithElysiumTransaction_ElysiumTransactionPropertyShouldNotBeNull()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            await issuer.IssueManagedAsync(owner);

            var hash = this.Node.Generate(1).Single();
            var height = 103;

            // Act.
            var result1 = await this.Subject.GetBlockAsync(hash);
            var result2 = await this.Subject.GetBlockAsync(height);

            // Assert.
            Assert.NotNull(result1);
            Assert.NotNull(result2);

            Assert.Equal(result1!.GetHash(), result2!.GetHash());
            Assert.Equal(2, result1.Transactions.Count);

            var elysium1 = result1.Transactions[0].GetElysiumTransaction();
            var elysium2 = result1.Transactions[1].GetElysiumTransaction();

            Assert.Null(elysium1);
            Assert.NotNull(elysium2);

            Assert.Equal(CreateManagedPropertyV0.StaticId, elysium2!.Id);
        }

        [Fact]
        public async Task GetBlockHeaderAsync_WithInvalidHash_ShouldReturnNull()
        {
            var header = await this.Subject.GetBlockHeaderAsync(uint256.One);

            Assert.Null(header);
        }

        [Fact]
        public async Task GetBlockHeaderAsync_WithInvalidHeight_ShouldReturnNull()
        {
            var header = await this.Subject.GetBlockHeaderAsync(1);

            Assert.Null(header);
        }

        [Fact]
        public Task GetBlockHeaderAsync_WithNegativeHeight_ShouldThrow()
        {
            return Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "height",
                () => Subject.GetBlockHeaderAsync(-1));
        }

        [Fact]
        public async Task GetBlockHeaderAsync_WithGenesisHash_ShouldReturnGenesisHeader()
        {
            var header = await Subject.GetBlockHeaderAsync(this.Network.GenesisHash);

            Assert.NotNull(header);
            Assert.Equal(this.Network.GenesisHash, header!.GetHash());
        }

        [Fact]
        public async Task GetBlockHeaderAsync_WithGenesisHeight_ShouldReturnGenesisHeader()
        {
            var header = await Subject.GetBlockHeaderAsync(0);

            Assert.NotNull(header);
            Assert.Equal(this.Network.GenesisHash, header!.GetHash());
        }

        [Fact]
        public async Task GetChainInfoAsync_WhenInvoke_ShouldReturnCurrentChainInfo()
        {
            // Arrange.
            var blocks = Node.Generate(3);

            // Act.
            var info = await Subject.GetChainInfoAsync(CancellationToken.None);

            // Assert.
            Assert.Equal(NetworkType.Regtest, info.Chain.NetworkType);
            Assert.Equal(3U, info.Blocks);
            Assert.Equal(3U, info.Headers);
            Assert.Equal(blocks.Last(), info.BestBlockHash);
            Assert.Equal(0U, info.Difficulty);
            Assert.NotEqual(0U, info.MedianTime);
            Assert.NotEqual(default(float), info.VerificationProgress);
            Assert.False(info.InitialBlockDownload);
            Assert.NotEqual(uint256.Zero, info.ChainWork);
            Assert.Equal(0U, info.SizeOnDisk);
            Assert.False(info.Pruned);
            Assert.NotEmpty(info.SoftForks);
            Assert.NotEmpty(info.Bip9SoftForks);

            Assert.NotEmpty(info.Bip9SoftForks[0].Name);
            Assert.NotEmpty(info.Bip9SoftForks[0].Status);
            Assert.NotEqual(default(DateTimeOffset), info.Bip9SoftForks[0].StartTime);
            Assert.NotEqual(default(DateTimeOffset), info.Bip9SoftForks[0].Timeout);
        }

        [Fact]
        public async Task GetTransactionAsync_WithInvalidHash_ShouldReturnNull()
        {
            var result = await this.Subject.GetTransactionAsync(uint256.One);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTransactionAsync_WithValidHash_ShouldReturnCorrespondingTransaction()
        {
            // Arrange.
            var owner = await this.GenerateNewAddressAsync();
            var issuer = new PropertyIssuer(this.Factory);

            this.Node.Generate(101);
            await this.FundAddressAsync(owner, Money.Coins(1));
            this.Node.Generate(1);

            var hash = uint256.Parse(await issuer.IssueManagedAsync(owner));

            // Act.
            var result = await this.Subject.GetTransactionAsync(hash);

            // Assert.
            Assert.NotNull(result);
            Assert.Equal(hash, result!.GetHash());
        }

        protected override ChainInformationClient CreateSubject()
        {
            return new ChainInformationClient(this.Factory, this.Node.CreateRPCClient());
        }
    }
}
