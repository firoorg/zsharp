namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class ElysiumTransactionTests
    {
        readonly ElysiumTransaction subject;

        public ElysiumTransactionTests()
        {
            this.subject = new ElysiumTransaction(uint256.One, 1, 2, "Simple Send", Money.Satoshis(1), true);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(-2L)]
        public void Constructor_WithNegativeFee_ShouldThrow(long fee)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "fee",
                () => new ElysiumTransaction(uint256.Zero, 0, 0, "abc", new Money(fee), false));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Null(this.subject.Confirmation);
            Assert.Equal(Money.Satoshis(1), this.subject.Fee);
            Assert.Equal(uint256.One, this.subject.Id);
            Assert.Equal("Simple Send", this.subject.Name);
            Assert.True(this.subject.Owned);
            Assert.Null(this.subject.ReferenceAddress);
            Assert.Null(this.subject.SendingAddress);
            Assert.Equal(1, this.subject.Type);
            Assert.Equal(2, this.subject.Version);
        }

        [Fact]
        public void Confirmation_WhenAssigned_ShouldUpdated()
        {
            var value = new ElysiumConfirmation(0, uint256.Zero, DateTime.Now, 0, 1, true);

            this.subject.Confirmation = value;

            Assert.Same(value, this.subject.Confirmation);
        }

        [Fact]
        public void ReferenceAddress_WhenAssigned_ShouldUpdated()
        {
            var value = TestAddress.Mainnet1;

            this.subject.ReferenceAddress = value;

            Assert.Same(value, this.subject.ReferenceAddress);
        }

        [Fact]
        public void SendingAddress_WhenAssigned_ShouldUpdated()
        {
            var value = TestAddress.Mainnet1;

            this.subject.SendingAddress = value;

            Assert.Same(value, this.subject.SendingAddress);
        }
    }
}
