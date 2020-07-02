using System;
using Xunit;
using Zsharp.Elysium;

namespace Zsharp.Rpc.Client.Tests
{
    public sealed class ElysiumBalanceTests
    {
        readonly ElysiumBalance subject;

        public ElysiumBalanceTests()
        {
            this.subject = new ElysiumBalance(new TokenAmount(1), new TokenAmount(2));
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithNegativeBalance_ShouldThrow(long balance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "balance",
                () => new ElysiumBalance(new TokenAmount(balance), TokenAmount.Zero));
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithNegativeReserved_ShouldThrow(long reserved)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "reserved",
                () => new ElysiumBalance(TokenAmount.Zero, new TokenAmount(reserved)));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(new TokenAmount(1), this.subject.Balance);
            Assert.Equal(new TokenAmount(2), this.subject.Reserved);
        }
    }
}
