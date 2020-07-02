namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Elysium;

    public sealed class TokenGrantHistoryTests
    {
        readonly TokenGrantHistory subject;

        public TokenGrantHistoryTests()
        {
            this.subject = new TokenGrantHistory(TokenGrantType.Revoke, uint256.One, new TokenAmount(10));
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithNegativeAmount_ShouldThrow(long amount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "amount",
                () => new TokenGrantHistory(TokenGrantType.Grant, uint256.Zero, new TokenAmount(amount)));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(new TokenAmount(10), this.subject.Amount);
            Assert.Equal(uint256.One, this.subject.Transaction);
            Assert.Equal(TokenGrantType.Revoke, this.subject.Type);
        }
    }
}
