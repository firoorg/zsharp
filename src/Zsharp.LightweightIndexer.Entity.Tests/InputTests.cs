namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class InputTests
    {
        readonly uint256 transaction;
        readonly uint256 spend;
        readonly Script script;
        readonly int sequence;
        readonly Input subject;

        public InputTests()
        {
            this.transaction = uint256.Parse("43e2c764a2457b55ac86639290b0b71dbd966bc1fc983527129a86bf58737c0f");
            this.spend = uint256.Parse("c9cb0150069c03102df1241ebab10b329ca8b54d41c7091527268ca789226bb8");
            this.script = Script.FromHex("47304402204e55fc102bc8e3470146a4f7552f89663074eb4276c6fd66ec985f89173411250220175c94a5886a0ebdfec1234bcdab4cf08657272dbb3ed9fe194847a59fabebe9012103d5d722325ee3a393ee2e883b25f254d43b9443f3b10598845a6c30fc0e1aae12");
            this.sequence = unchecked((int)4294967295);
            this.subject = new Input(this.transaction, 1, this.spend, 15, this.script, this.sequence);
        }

        [Fact]
        public void Constructor_WithNegativeIndex_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "index",
                () => new Input(this.transaction, -1, this.spend, 0, this.script, this.sequence));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(1, this.subject.Index);
            Assert.Equal(this.spend, this.subject.OutputHash);
            Assert.Equal(15, this.subject.OutputIndex);
            Assert.Equal(this.script, this.subject.Script);
            Assert.Equal(this.sequence, this.subject.Sequence);
            Assert.Equal(this.transaction, this.subject.TransactionHash);
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
            var other = new Input(
                this.subject.TransactionHash,
                this.subject.Index + 1,
                this.subject.OutputHash,
                this.subject.OutputIndex,
                this.subject.Script,
                this.subject.Sequence);

            var result = this.subject.CompareTo(other);

            Assert.True(result < 0);
        }

        [Fact]
        public void CompareTo_WithLowerIndex_ShouldReturnPositive()
        {
            var other = new Input(
                this.subject.TransactionHash,
                this.subject.Index - 1,
                this.subject.OutputHash,
                this.subject.OutputIndex,
                this.subject.Script,
                this.subject.Sequence);

            var result = this.subject.CompareTo(other);

            Assert.True(result > 0);
        }

        [Fact]
        public void CompareTo_WithSameIndex_ShouldReturnZero()
        {
            var other = new Input(
                this.subject.TransactionHash,
                this.subject.Index,
                this.subject.OutputHash,
                this.subject.OutputIndex,
                this.subject.Script,
                this.subject.Sequence);

            var result = this.subject.CompareTo(other);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Equals_WithUnequals_ShouldReturnFalse()
        {
            var tester = new StandardEqualityTester<Input>(
                this.subject,
                s => null,
                s => string.Empty,
                s => new Input(s.TransactionHash, 0, s.OutputHash, s.OutputIndex, s.Script, s.Sequence));

            Assert.DoesNotContain(true, tester);
        }

        [Fact]
        public void Equals_WithEquals_ShouldReturnTrue()
        {
            var tester = new StandardEqualityTester<Input>(
                this.subject,
                s => s,
                s => new Input(s.TransactionHash, s.Index, s.OutputHash, s.OutputIndex, s.Script, s.Sequence),
                s => new Input(uint256.One, s.Index, s.OutputHash, s.OutputIndex, s.Script, s.Sequence),
                s => new Input(s.TransactionHash, s.Index, uint256.One, s.OutputIndex, s.Script, s.Sequence),
                s => new Input(s.TransactionHash, s.Index, s.OutputHash, 0, s.Script, s.Sequence),
                s => new Input(s.TransactionHash, s.Index, s.OutputHash, s.OutputIndex, Script.Empty, s.Sequence),
                s => new Input(s.TransactionHash, s.Index, s.OutputHash, s.OutputIndex, s.Script, 0));

            Assert.DoesNotContain(false, tester);
        }

        [Fact]
        public void GetHashCode_WithDifferentIndex_ShouldReturnDifferentValue()
        {
            var other = new Input(
                this.subject.TransactionHash,
                0,
                this.subject.OutputHash,
                this.subject.OutputIndex,
                this.subject.Script,
                this.subject.Sequence);

            Assert.NotEqual(this.subject.GetHashCode(), other.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WithSameIndex_ShouldReturnSameValue()
        {
            var other = new Input(
                this.subject.TransactionHash,
                this.subject.Index,
                this.subject.OutputHash,
                this.subject.OutputIndex,
                this.subject.Script,
                this.subject.Sequence);

            Assert.Equal(this.subject.GetHashCode(), other.GetHashCode());
        }
    }
}
