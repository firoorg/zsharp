namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;
    using Block = Zsharp.LightweightIndexer.Entity.Block;
    using Transaction = Zsharp.LightweightIndexer.Entity.Transaction;

    public sealed class BlockTransactionTests
    {
        readonly uint256 block;
        readonly uint256 transaction;
        readonly BlockTransaction subject;

        public BlockTransactionTests()
        {
            this.block = uint256.Parse("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd66");
            this.transaction = uint256.Parse("9d80e018f3a876fe050c8efc9eff71504316491c30f333931979261e3d888807");
            this.subject = new BlockTransaction(this.block, 1, this.transaction);
        }

        [Fact]
        public void Constructor_WithNegativeIndex_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "index",
                () => new BlockTransaction(this.block, -1, this.transaction));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Null(this.subject.Block);
            Assert.Equal(this.block, this.subject.BlockHash);
            Assert.Equal(1, this.subject.Index);
            Assert.Null(this.subject.Transaction);
            Assert.Equal(this.transaction, this.subject.TransactionHash);
        }

        [Fact]
        public void CompareTo_WithNull_ShouldReturnPositive()
        {
            var result = this.subject.CompareTo(null);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithGreaterBlockHash_ShouldReturnNegative()
        {
            var other = new BlockTransaction(
                uint256.Parse("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd67"),
                this.subject.Index,
                this.subject.TransactionHash);

            var result = this.subject.CompareTo(other);

            Assert.True(result < 0);
        }

        [Fact]
        public void CompareTo_LowerBlockHash_ShouldReturnPositive()
        {
            var other = new BlockTransaction(
                uint256.Parse("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd65"),
                this.subject.Index,
                this.subject.TransactionHash);

            var result = this.subject.CompareTo(other);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithGreaterIndex_ShouldReturnNegative()
        {
            var other = new BlockTransaction(
                this.subject.BlockHash,
                this.subject.Index + 1,
                this.subject.TransactionHash);

            var result = this.subject.CompareTo(other);

            Assert.True(result < 0);
        }

        [Fact]
        public void CompareTo_WithLowerIndex_ShouldGreater()
        {
            var other = new BlockTransaction(
                this.subject.BlockHash,
                this.subject.Index - 1,
                this.subject.TransactionHash);

            var result = this.subject.CompareTo(other);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithSameBlockHashAndIndex_ShouldReturnZero()
        {
            var other = new BlockTransaction(
                this.subject.BlockHash,
                this.subject.Index,
                this.subject.TransactionHash);

            var result = this.subject.CompareTo(other);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Equals_WithUnequals_ShouldReturnFalse()
        {
            var tester = new StandardEqualityTester<BlockTransaction>(
                this.subject,
                s => null,
                s => string.Empty,
                s => new BlockTransaction(uint256.One, s.Index, s.TransactionHash),
                s => new BlockTransaction(s.BlockHash, 0, s.TransactionHash));

            Assert.DoesNotContain(true, tester);
        }

        [Fact]
        public void Equals_WithEquals_ShouldReturnTrue()
        {
            var tester = new StandardEqualityTester<BlockTransaction>(
                this.subject,
                s => s,
                s => new BlockTransaction(s.BlockHash, s.Index, s.TransactionHash),
                s => new BlockTransaction(s.BlockHash, s.Index, uint256.One),
                s => new BlockTransaction(s.BlockHash, s.Index, s.TransactionHash)
                {
                    Block = new Block(0, uint256.Zero, DateTime.Now, 0, Target.Difficulty1, 0, uint256.Zero),
                },
                s => new BlockTransaction(s.BlockHash, s.Index, s.TransactionHash)
                {
                    Transaction = new Transaction(uint256.Zero, 0, 0),
                });

            Assert.DoesNotContain(false, tester);
        }

        [Theory]
        [InlineData("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd66", 0)]
        [InlineData("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd67", 1)]
        [InlineData("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd67", 0)]
        public void GetHashCode_WithDifferentBlockHashAndIndex_ShouldReturnDifferentValue(string blockHash, int index)
        {
            var other = new BlockTransaction(uint256.Parse(blockHash), index, this.subject.TransactionHash);

            Assert.NotEqual(this.subject.GetHashCode(), other.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithSameBlockHashAndIndex_ShouldReturnSameValue()
        {
            var other = new BlockTransaction(this.subject.BlockHash, this.subject.Index, this.subject.TransactionHash);

            Assert.Equal(this.subject.GetHashCode(), other.GetHashCode());
        }
    }
}
