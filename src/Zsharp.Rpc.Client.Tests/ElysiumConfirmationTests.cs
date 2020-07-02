namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using NBitcoin;
    using Xunit;

    public sealed class ElysiumConfirmationTests
    {
        readonly ElysiumConfirmation subject;

        public ElysiumConfirmationTests()
        {
            this.subject = new ElysiumConfirmation(0, uint256.One, new DateTime(2020, 7, 31), 0, 1, true);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Constructor_WithNegativeBlock_ShouldThrow(int block)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "block",
                () => new ElysiumConfirmation(block, uint256.Zero, DateTime.Now, 0, 1, true));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void Constructor_WithNegativeIndex_ShouldThrow(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "index",
                () => new ElysiumConfirmation(0, uint256.Zero, DateTime.Now, index, 1, false));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidCount_ShouldThrow(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "count",
                () => new ElysiumConfirmation(0, uint256.Zero, DateTime.Now, 0, count, true));
        }

        [Fact]
        public void Constructor_WithValidArguments_ShouldInitializeProperties()
        {
            Assert.Equal(0, this.subject.Block);
            Assert.Equal(uint256.One, this.subject.BlockHash);
            Assert.Equal(0, this.subject.BlockIndex);
            Assert.Equal(new DateTime(2020, 7, 31), this.subject.BlockTime);
            Assert.Equal(1, this.subject.Count);
            Assert.Null(this.subject.InvalidReason);
            Assert.True(this.subject.Valid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abc")]
        public void InvalidReason_WhenAssign_ShouldUpdatedWithThatValue(string? value)
        {
            this.subject.InvalidReason = value;

            Assert.Equal(value, this.subject.InvalidReason);
        }
    }
}
