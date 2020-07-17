namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class OutputTests
    {
        readonly uint256 transaction;
        readonly Script script;
        readonly Output subject;

        public OutputTests()
        {
            this.transaction = uint256.Parse("3449fb1d52c3b88060f6a66de5e2c9d001017af4b9f7078e04c9a731f0fefba8");
            this.script = Script.FromHex("76a91451851a4156a1d1c447c740d228c93c733dfbd13e88ac");
            this.subject = new Output(this.transaction, 1, this.script, 7500000000);
        }

        [Fact]
        public void Constructor_WithNegativeIndex_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "index",
                () => new Output(this.transaction, -1, this.script, 7500000000));
        }

        [Fact]
        public void Constructor_WithNegativeValue_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("value", () => new Output(this.transaction, 1, this.script, -1));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(1, this.subject.Index);
            Assert.Equal(this.script, this.subject.Script);
            Assert.Equal(this.transaction, this.subject.TransactionHash);
            Assert.Equal(7500000000, this.subject.Value);
        }

        [Fact]
        public void CompareTo_WithNull_ShouldReturnPositive()
        {
            var result = this.subject.CompareTo(null);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithGreaterIndex_ShouldReturnNegative()
        {
            var other = new Output(
                this.subject.TransactionHash,
                this.subject.Index + 1,
                this.subject.Script,
                this.subject.Value);

            var result = this.subject.CompareTo(other);

            Assert.True(result < 0);
        }

        [Fact]
        public void CompareTo_WithLowerIndex_ShouldReturnPositive()
        {
            var other = new Output(
                this.subject.TransactionHash,
                this.subject.Index - 1,
                this.subject.Script,
                this.subject.Value);

            var result = this.subject.CompareTo(other);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithSameIndex_ShouldReturnZero()
        {
            var other = new Output(
                this.subject.TransactionHash,
                this.subject.Index,
                this.subject.Script,
                this.subject.Value);

            var result = this.subject.CompareTo(other);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Equals_WithUnequals_ShouldReturnFalse()
        {
            var tester = new StandardEqualityTester<Output>(
                this.subject,
                s => null,
                s => string.Empty,
                s => new Output(s.TransactionHash, 0, s.Script, s.Value));

            Assert.DoesNotContain(true, tester);
        }

        [Fact]
        public void Equals_WithEquals_ShouldReturnTrue()
        {
            var tester = new StandardEqualityTester<Output>(
                this.subject,
                s => s,
                s => new Output(s.TransactionHash, s.Index, s.Script, s.Value),
                s => new Output(uint256.One, s.Index, s.Script, s.Value),
                s => new Output(s.TransactionHash, s.Index, Script.Empty, s.Value),
                s => new Output(s.TransactionHash, s.Index, s.Script, 0));

            Assert.DoesNotContain(false, tester);
        }

        [Fact]
        public void GetHashCode_WithDifferentIndex_ShouldReturnDifferentValue()
        {
            var other = new Output(
                this.subject.TransactionHash,
                0,
                this.subject.Script,
                this.subject.Value);

            Assert.NotEqual(this.subject.GetHashCode(), other.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithSameIndex_ShouldReturnSameValue()
        {
            var other = new Output(
                this.subject.TransactionHash,
                this.subject.Index,
                this.subject.Script,
                this.subject.Value);

            Assert.Equal(this.subject.GetHashCode(), other.GetHashCode());
        }
    }
}
