namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class ElysiumTransactionTests
    {
        readonly ElysiumTransaction subject;

        public ElysiumTransactionTests()
        {
            this.subject = new ElysiumTransaction(
                uint256.One,
                "a8ULhhDgfdSiXJhSZVdhb8EuDc6R3ogsaM",
                "a1xFVCnBvLKYdf48ApLn6rgwsKNo6R4nis",
                new byte[] { 0x00, 0x01 });
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal("a1xFVCnBvLKYdf48ApLn6rgwsKNo6R4nis", this.subject.Receiver);
            Assert.Equal("a8ULhhDgfdSiXJhSZVdhb8EuDc6R3ogsaM", this.subject.Sender);
            Assert.Equal(new byte[] { 0x00, 0x01 }, this.subject.Serialized);
            Assert.Equal(uint256.One, this.subject.TransactionHash);
        }
    }
}
