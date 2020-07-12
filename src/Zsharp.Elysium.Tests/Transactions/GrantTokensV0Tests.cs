namespace Zsharp.Elysium.Tests.Transactions
{
    using System;
    using Xunit;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Testing;

    public sealed class GrantTokensV0Tests
    {
        readonly GrantTokensV0 subject;

        public GrantTokensV0Tests()
        {
            this.subject = new GrantTokensV0(
                TestAddress.Regtest1,
                TestAddress.Regtest2,
                new PropertyId(1),
                new TokenAmount(10));
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithInvalidAmount_ShouldThrow(long amount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "amount",
                () => new GrantTokensV0(TestAddress.Regtest1, null, new PropertyId(1), new TokenAmount(amount)));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(10, this.subject.Amount.Value);
            Assert.Equal(GrantTokensV0.StaticId, this.subject.Id);
            Assert.Equal(1, this.subject.Property.Value);
            Assert.Equal(TestAddress.Regtest2, this.subject.Receiver);
            Assert.Equal(TestAddress.Regtest1, this.subject.Sender);
            Assert.Equal(0, this.subject.Version);
        }
    }
}
