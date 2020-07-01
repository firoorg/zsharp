namespace Zsharp.Elysium.Tests
{
    using Xunit;
    using Zsharp.Testing;

    public sealed class TransactionTests
    {
        [Fact]
        public void Constructor_WithNullSender_ShouldSuccess()
        {
            var tx = new FakeTransaction(null, null);

            Assert.Null(tx.Sender);
        }

        [Fact]
        public void Constructor_WithNonNullSender_ShouldSuccess()
        {
            var sender = TestAddress.Regtest1;
            var tx = new FakeTransaction(sender, null);

            Assert.Same(sender, tx.Sender);
        }

        [Fact]
        public void Constructor_WithNullReceiver_ShouldSuccess()
        {
            var tx = new FakeTransaction(null, null);

            Assert.Null(tx.Receiver);
        }

        [Fact]
        public void Constructor_WithNonNullReceiver_ShouldSuccess()
        {
            var receiver = TestAddress.Regtest1;
            var tx = new FakeTransaction(null, receiver);

            Assert.Same(receiver, tx.Receiver);
        }

        [Theory]
        [InlineData(Transaction.MinId)]
        [InlineData(Transaction.MaxId)]
        public void IsValidId_WithValidId_ShouldReturnTrue(int id)
        {
            Assert.True(Transaction.IsValidId(id));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(65536)]
        public void IsValidId_WithInvalidId_ShouldReturnFalse(int id)
        {
            Assert.False(Transaction.IsValidId(id));
        }

        [Theory]
        [InlineData(Transaction.MinVersion)]
        [InlineData(Transaction.MaxVersion)]
        public void IsValidVersion_WithValidVersion_ShouldReturnTrue(int version)
        {
            Assert.True(Transaction.IsValidVersion(version));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(65536)]
        public void IsValidVersion_WithInvalidVersion_ShouldReturnFalse(int version)
        {
            Assert.False(Transaction.IsValidVersion(version));
        }
    }
}
