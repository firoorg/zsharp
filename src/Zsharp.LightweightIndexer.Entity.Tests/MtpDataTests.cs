namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class MtpDataTests
    {
        readonly uint256 block;
        readonly uint256 hash;
        readonly uint256 reserved1;
        readonly uint256 reserved2;
        readonly MtpData subject;

        public MtpDataTests()
        {
            this.block = uint256.Parse("8837eaa25fca59b88dddcb3f89d68a6a2400d62b1e6985d30567a72d7f4cc191");
            this.hash = uint256.Parse("7bb82cb092cac59974bcbbec1828a70def4334a51d5ace60a61dff15805584ff");
            this.reserved1 = uint256.Parse("adb0e22904c14d6a73fc083da5edfaeb36325e1126fdc53c918681123c61790e");
            this.reserved2 = uint256.Parse("6002cb7a2ffda2365d46f651691978ad4afd68b3bf4e8c28bc422b9706ddf620");
            this.subject = new MtpData(this.block, this.hash, 100, this.reserved1, this.reserved2);
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(this.block, this.subject.BlockHash);
            Assert.Equal(this.hash, this.subject.Hash);
            Assert.Equal(this.reserved1, this.subject.Reserved1);
            Assert.Equal(this.reserved2, this.subject.Reserved2);
            Assert.Equal(100, this.subject.Version);
        }
    }
}
