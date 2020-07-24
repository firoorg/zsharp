namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NBitcoin;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Elysium.TransactionSerializers;
    using Zsharp.Entity;
    using Zsharp.Testing;
    using Zsharp.Zcoin;
    using DbContext = Zsharp.LightweightIndexer.Entity.DbContext;

    public class BlockRepositoryTests : IDisposable
    {
        readonly Network network;
        readonly IDbContextFactory<DbContext> db;
        readonly BlockRepository subject;

        public BlockRepositoryTests()
            : this(new FakeDbContextFactory())
        {
        }

        protected BlockRepositoryTests(IDbContextFactory<DbContext> db)
        {
            try
            {
                this.network = Networks.Default.Mainnet;
                this.db = db;

                this.subject = new BlockRepository(
                    this.network,
                    this.db,
                    new TransactionSerializer(
                        new ITransactionPayloadSerializer[]
                        {
                            new CreateManagedPropertySerializer(),
                            new SimpleSendSerializer(),
                        }));

                using (var context = db.CreateAsync().AsTask().Result)
                {
                    context.Database.EnsureCreated();
                }
            }
            catch
            {
                if (db is IDisposable d)
                {
                    d.Dispose();
                }
                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            using (var db = this.db.CreateAsync().AsTask().Result)
            {
                db.Blocks.RemoveRange(db.Blocks);
                db.Transactions.RemoveRange(db.Transactions);
                db.SaveChanges();
            }

            if (this.db is IDisposable d)
            {
                d.Dispose();
            }
        }

        [Fact]
        public async Task AddBlockAsync_WithNegativeHeight_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "height",
                () => this.subject.AddBlockAsync(TestBlock.Mainnet117563, -1).AsTask());
        }

        [Fact]
        public async Task AddBlockAsync_WithValidArgs_ShouldAbleToRetrieveThatBlock()
        {
            // Act.
            await this.subject.AddBlockAsync(TestBlock.Mainnet117193, 117193);
            await this.subject.AddBlockAsync(TestBlock.Mainnet117194, 117194);

            await this.subject.AddBlockAsync(TestBlock.Mainnet117563, 117563);
            await this.subject.AddBlockAsync(TestBlock.Mainnet117564, 117564);

            await this.subject.AddBlockAsync(TestBlock.Mainnet286713, 286713);
            await this.subject.AddBlockAsync(TestBlock.Mainnet286714, 286714);

            // Assert.
            var elysium = await this.subject.GetBlockAsync(117194);

            Assert.NotNull(elysium);
            Assert.Equal(TestBlock.Mainnet117194.GetHash(), elysium!.GetHash());
            Assert.Equal(8, elysium.Transactions.Count);
            Assert.Equal(TransactionType.Normal, elysium.Transactions[0].GetTransactionType());
            Assert.Equal(1U, elysium.Transactions[0].Version);
            Assert.Null(elysium.Transactions[0].GetElysiumTransaction());
            Assert.Equal(TransactionType.Normal, elysium.Transactions[5].GetTransactionType());
            Assert.Equal(1U, elysium.Transactions[5].Version);
            Assert.IsType<CreateManagedPropertyV0>(elysium.Transactions[5].GetElysiumTransaction());
            Assert.Equal(TransactionType.Normal, elysium.Transactions[6].GetTransactionType());
            Assert.Equal(1U, elysium.Transactions[6].Version);
            Assert.Null(elysium.Transactions[6].GetElysiumTransaction());

            var (mtp, height) = await this.subject.GetBlockAsync(TestBlock.Mainnet117564.GetHash());

            Assert.NotNull(mtp);
            Assert.Equal(TestBlock.Mainnet117564.GetHash(), mtp!.GetHash());
            Assert.Equal(117564, height);
            Assert.Equal(3, mtp.Transactions.Count);
            Assert.Equal(TransactionType.Normal, mtp.Transactions[0].GetTransactionType());
            Assert.Equal(1U, mtp.Transactions[0].Version);
            Assert.Null(mtp.Transactions[0].GetElysiumTransaction());
            Assert.Equal(TransactionType.Normal, mtp.Transactions[1].GetTransactionType());
            Assert.Equal(1U, mtp.Transactions[1].Version);
            Assert.Null(mtp.Transactions[1].GetElysiumTransaction());

            var evo = await this.subject.GetBlockAsync(286714);

            Assert.NotNull(evo);
            Assert.Equal(TestBlock.Mainnet286714.GetHash(), evo!.GetHash());
            Assert.Equal(2, evo.Transactions.Count);
            Assert.Equal(TransactionType.Coinbase, evo.Transactions[0].GetTransactionType());
            Assert.Equal(3U, evo.Transactions[0].Version);
            Assert.Null(evo.Transactions[0].GetElysiumTransaction());
            Assert.Equal(TransactionType.QuorumCommitment, evo.Transactions[1].GetTransactionType());
            Assert.Equal(3U, evo.Transactions[1].Version);
            Assert.Null(evo.Transactions[1].GetElysiumTransaction());
        }

        [Fact]
        public async Task GetBlockAsync_WithNegativeHeight_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "height",
                () => this.subject.GetBlockAsync(-1).AsTask());
        }

        [Fact]
        public async Task GetBlockAsync_WithInvalidHash_ShouldReturnNull()
        {
            // Arrange.
            await this.subject.AddBlockAsync(TestBlock.Mainnet0, 0);

            // Act.
            var (block, height) = await this.subject.GetBlockAsync(uint256.One);

            // Assert.
            Assert.Null(block);
            Assert.Equal(-1, height);
        }

        [Fact]
        public async Task GetBlockAsync_WithInvalidHeight_ShouldReturnNull()
        {
            // Arrange.
            await this.subject.AddBlockAsync(TestBlock.Mainnet0, 0);

            // Act.
            var block = await this.subject.GetBlockAsync(1);

            // Assert.
            Assert.Null(block);
        }

        [Fact]
        public async Task GetLatestsBlocksAsync_WithNegativeCount_ShouldThrow()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "count",
                () => this.subject.GetLatestsBlocksAsync(-1).AsTask());
        }

        [Fact]
        public async Task GetLatestsBlocksAsync_NoAnyBlocks_ShouldReturnEmpty()
        {
            var (blocks, height) = await this.subject.GetLatestsBlocksAsync(1);

            Assert.Empty(blocks);
            Assert.Equal(-1, height);
        }

        [Fact]
        public async Task GetLatestsBlocksAsync_WithZeroCount_ShouldReturnEmpty()
        {
            await this.subject.AddBlockAsync(TestBlock.Mainnet0, 0);

            var (blocks, height) = await this.subject.GetLatestsBlocksAsync(0);

            Assert.Empty(blocks);
            Assert.Equal(-1, height);
        }

        [Fact]
        public async Task GetLatestsBlocksAsync_WithPositiveCount_ShouldReturnBlocksUpToCount()
        {
            // Arrange.
            await this.subject.AddBlockAsync(TestBlock.Mainnet0, 0);
            await this.subject.AddBlockAsync(TestBlock.Mainnet1, 1);

            // Act.
            var (blocks, height) = await this.subject.GetLatestsBlocksAsync(3);

            // Assert.
            Assert.Equal(2, blocks.Count());
            Assert.Equal(1, height);

            Assert.Equal(TestBlock.Mainnet1.GetHash(), blocks.ElementAt(0).GetHash());
            Assert.Equal(TestBlock.Mainnet0.GetHash(), blocks.ElementAt(1).GetHash());
        }

        [Fact]
        public async Task GetTransactionAsync_WithInvalidHash_ShouldReturnNull()
        {
            await this.subject.AddBlockAsync(TestBlock.Mainnet0, 0);

            var tx = await this.subject.GetTransactionAsync(uint256.One);

            Assert.Null(tx);
        }

        [Fact]
        public async Task GetTransactionAsync_WithValidHash_ShouldReturnCorrespondingTransaction()
        {
            await this.subject.AddBlockAsync(TestBlock.Mainnet1, 1);

            var tx = await this.subject.GetTransactionAsync(
                uint256.Parse("98f7ecc5b17fa795ceb45809918e726d50a42fdb9207f40d8a0fe0dcf0f57b70"));

            Assert.NotNull(tx);
            Assert.Equal(TestBlock.Mainnet1.Transactions[0].GetHash(), tx!.GetHash());
        }

        [Fact]
        public async Task RemoveLastBlockAsync_WithSomeBlocks_ShouldRemoveLastBlockAndAnyAssociatedData()
        {
            // Arrange.
            await this.subject.AddBlockAsync(TestBlock.Mainnet212063, 212063);
            await this.subject.AddBlockAsync(TestBlock.Mainnet212064, 212064);

            // Act.
            await this.subject.RemoveLastBlockAsync();

            // Assert.
            await using (var db = await this.db.CreateAsync())
            {
                var blocks = await db.Blocks.ToListAsync();
                var joins = await db.BlockTransactions.ToListAsync();
                var elysiums = await db.ElysiumTransactions.ToListAsync();
                var inputs = await db.Inputs.ToListAsync();
                var mtps = await db.MtpData.ToListAsync();
                var outputs = await db.Outputs.ToListAsync();
                var transactions = await db.Transactions.ToListAsync();

                Assert.Single(blocks);
                Assert.Equal(212063, blocks[0].Height);
                Assert.Equal(9, joins.Count);
                Assert.Empty(elysiums);
                Assert.Equal(1 + 101 + 101 + 1 + 354 + 166 + 1 + 1 + 1, inputs.Count);
                Assert.Single(mtps);
                Assert.Equal(TestBlock.Mainnet212063.GetHash(), mtps[0].BlockHash);
                Assert.Equal(7 + 2 + 2 + 2 + 2 + 2 + 2 + 1 + 1, outputs.Count);
                Assert.Equal(9, transactions.Count);
            }
        }
    }
}
