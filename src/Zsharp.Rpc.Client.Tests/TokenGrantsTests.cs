namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using NBitcoin;
    using Xunit;
    using Zsharp.Elysium;
    using Zsharp.Testing;

    public sealed class TokenGrantsTests
    {
        readonly IEnumerable<TokenGrantHistory> histories;
        readonly TokenGrants subject;

        public TokenGrantsTests()
        {
            this.histories = new[]
            {
                new TokenGrantHistory(TokenGrantType.Grant, uint256.Zero, new TokenAmount(1000))
            };

            this.subject = new TokenGrants(
                new PropertyId(1),
                "Test",
                TestAddress.Regtest1,
                uint256.One,
                new TokenAmount(1000),
                this.histories);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithNegativeTotalTokens_ShouldThrow(long totalTokens)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "totalTokens",
                () => new TokenGrants(
                    this.subject.PropertyId,
                    this.subject.PropertyName,
                    this.subject.PropertyIssuer,
                    this.subject.PropertyCreationTransaction,
                    new TokenAmount(totalTokens),
                    this.subject.GrantHistories));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(uint256.One, this.subject.PropertyCreationTransaction);
            Assert.Equal(new PropertyId(1), this.subject.PropertyId);
            Assert.Equal(TestAddress.Regtest1, this.subject.PropertyIssuer);
            Assert.Equal("Test", this.subject.PropertyName);
            Assert.Equal(new TokenAmount(1000), this.subject.TotalTokens);
            Assert.Same(this.histories, this.subject.GrantHistories);
        }
    }
}
