namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using NBitcoin;
    using Xunit;
    using Transaction = Zsharp.LightweightIndexer.Entity.Transaction;

    public sealed class TransactionTests
    {
        readonly uint256 hash;
        readonly Transaction subject;

        public TransactionTests()
        {
            this.hash = uint256.Parse("b247fc82bae4a5b800734fff13c998329abd5f7babb2d1c4d51d0a48e9fc3eec");
            this.subject = new Transaction(this.hash, 3, 1);
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Empty(this.subject.Blocks);
            Assert.Null(this.subject.Elysium);
            Assert.Null(this.subject.Extra);
            Assert.Equal(this.hash, this.subject.Hash);
            Assert.Empty(this.subject.Inputs);
            Assert.Equal(1, this.subject.LockTime);
            Assert.Empty(this.subject.Outputs);
            Assert.Equal(3, this.subject.Version);
        }
    }
}
