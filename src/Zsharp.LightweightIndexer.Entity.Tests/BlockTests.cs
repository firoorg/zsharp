namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;
    using Block = Zsharp.LightweightIndexer.Entity.Block;

    public sealed class BlockTests
    {
        readonly uint256 hash;
        readonly DateTime time;
        readonly Target target;
        readonly uint256 merkleRoot;
        readonly Block subject;

        public BlockTests()
        {
            this.hash = uint256.Parse("046217b009a488569697af675d623bb9fc438cbac2a762efae249239f43bcd66");
            this.time = DateTimeOffset.FromUnixTimeSeconds(1594993633).LocalDateTime;
            this.target = new Target(new byte[] { 0x1B, 0x0D, 0x6E, 0x26 });
            this.merkleRoot = uint256.Parse("9d80e018f3a876fe050c8efc9eff71504316491c30f333931979261e3d888807");
            this.subject = new Block(285504, this.hash, this.time, 0x20001000, this.target, 380010823, this.merkleRoot);
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(this.hash, this.subject.Hash);
            Assert.Equal(285504, this.subject.Height);
            Assert.Equal(this.merkleRoot, this.subject.MerkleRoot);
            Assert.Null(this.subject.MtpData);
            Assert.Equal(380010823, this.subject.Nonce);
            Assert.Equal(this.target, this.subject.Target);
            Assert.Equal(this.time, this.subject.Time);
            Assert.Empty(this.subject.Transactions);
            Assert.Equal(0x20001000, this.subject.Version);
        }

        [Fact]
        public void CompareTo_WithNull_ShouldReturnPositive()
        {
            var result = this.subject.CompareTo(null);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithGreaterHeight_ShouldReturnNegative()
        {
            var other = new Block(
                this.subject.Height + 1,
                this.subject.Hash,
                this.subject.Time,
                this.subject.Version,
                this.subject.Target,
                this.subject.Nonce,
                this.subject.MerkleRoot);

            var result = this.subject.CompareTo(other);

            Assert.True(result < 0);
        }

        [Fact]
        public void CompareTo_WithLowerHeight_ShouldReturnPositive()
        {
            var other = new Block(
                this.subject.Height - 1,
                this.subject.Hash,
                this.subject.Time,
                this.subject.Version,
                this.subject.Target,
                this.subject.Nonce,
                this.subject.MerkleRoot);

            var result = this.subject.CompareTo(other);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithSameHeight_ShouldReturnZero()
        {
            var other = new Block(
                this.subject.Height,
                this.subject.Hash,
                this.subject.Time,
                this.subject.Version,
                this.subject.Target,
                this.subject.Nonce,
                this.subject.MerkleRoot);

            var result = this.subject.CompareTo(other);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Equals_WithUnequals_ShouldReturnFalse()
        {
            var tester = new StandardEqualityTester<Block>(
                this.subject,
                s => null,
                s => string.Empty,
                s => new Block(s.Height + 1, s.Hash, s.Time, s.Version, s.Target, s.Nonce, s.MerkleRoot));

            Assert.DoesNotContain(true, tester);
        }

        [Fact]
        public void Equals_WithEquals_ShouldReturnTrue()
        {
            var tester = new StandardEqualityTester<Block>(
                this.subject,
                s => s,
                s => new Block(s.Height, s.Hash, s.Time, s.Version, s.Target, s.Nonce, s.MerkleRoot),
                s => new Block(s.Height, uint256.One, s.Time, s.Version, s.Target, s.Nonce, s.MerkleRoot),
                s => new Block(s.Height, s.Hash, DateTime.Now, s.Version, s.Target, s.Nonce, s.MerkleRoot),
                s => new Block(s.Height, s.Hash, s.Time, 10, s.Target, s.Nonce, s.MerkleRoot),
                s => new Block(s.Height, s.Hash, s.Time, s.Version, Target.Difficulty1, s.Nonce, s.MerkleRoot),
                s => new Block(s.Height, s.Hash, s.Time, s.Version, s.Target, 100, s.MerkleRoot),
                s => new Block(s.Height, s.Hash, s.Time, s.Version, s.Target, s.Nonce, uint256.One),
                s => new Block(s.Height, s.Hash, s.Time, s.Version, s.Target, s.Nonce, s.MerkleRoot)
                {
                    MtpData = new MtpData(s.Hash, uint256.One, 100, uint256.Zero, uint256.Zero),
                },
                s =>
                {
                    var o = new Block(s.Height, s.Hash, s.Time, s.Version, s.Target, s.Nonce, s.MerkleRoot);
                    o.Transactions.Add(new BlockTransaction(s.Hash, 0, uint256.One));
                    return o;
                });

            Assert.DoesNotContain(false, tester);
        }

        [Fact]
        public void GetHashCode_WithSameHeight_ShouldReturnSameValue()
        {
            var other = new Block(
                this.subject.Height,
                this.subject.Hash,
                this.subject.Time,
                this.subject.Version,
                this.subject.Target,
                this.subject.Nonce,
                this.subject.MerkleRoot);

            Assert.Equal(this.subject.GetHashCode(), other.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithDifferentHeight_ShouldReturnDifferentValue()
        {
            var other = new Block(
                this.subject.Height + 1,
                this.subject.Hash,
                this.subject.Time,
                this.subject.Version,
                this.subject.Target,
                this.subject.Nonce,
                this.subject.MerkleRoot);

            Assert.NotEqual(this.subject.GetHashCode(), other.GetHashCode());
        }
    }
}
