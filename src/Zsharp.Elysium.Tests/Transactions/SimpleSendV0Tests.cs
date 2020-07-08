namespace Zsharp.Elysium.Tests.Transactions
{
    using System;
    using Xunit;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Testing;

    public sealed class SimpleSendV0Tests
    {
        readonly PropertyId property;
        readonly TokenAmount amount;

        public SimpleSendV0Tests()
        {
            this.property = new PropertyId(3);
            this.amount = new TokenAmount(9);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        public void Constructor_WithInvalidAmount_ShouldThrow(long value)
        {
            var amount = new TokenAmount(value);

            Assert.Throws<ArgumentOutOfRangeException>(
                "amount",
                () => new SimpleSendV0(TestAddress.Regtest1, null, this.property, amount));
        }

        [Fact]
        public void Constructor_WhenSuccess_ShouldInitializeProperties()
        {
            var tx = new SimpleSendV0(TestAddress.Regtest1, TestAddress.Regtest2, this.property, this.amount);

            Assert.Equal(SimpleSendV0.StaticId, tx.Id);
            Assert.Equal(0, tx.Version);
            Assert.Equal(TestAddress.Regtest1, tx.Sender);
            Assert.Equal(TestAddress.Regtest2, tx.Receiver);
            Assert.Equal(this.property, tx.Property);
            Assert.Equal(this.amount, tx.Amount);
        }
    }
}
