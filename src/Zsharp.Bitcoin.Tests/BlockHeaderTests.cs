namespace Zsharp.Bitcoin.Tests
{
    using System;
    using NBitcoin;
    using Xunit;
    using Zsharp.Zcoin;

    public sealed class BlockHeaderTests
    {
        readonly BlockHeader subject;

        public BlockHeaderTests()
        {
            this.subject = Networks.Default.Mainnet.Consensus.ConsensusFactory.CreateBlockHeader();
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Null(this.subject.GetMtpData());
            Assert.Equal(uint256.Zero, this.subject.GetMtpHash());
            Assert.Equal(0x1000, this.subject.GetMtpVersion());
            Assert.Equal(uint256.Zero, this.subject.GetReserved1());
            Assert.Equal(uint256.Zero, this.subject.GetReserved2());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1414776286)]
        [InlineData(1544443199)]
        public void IsMtp_WithTimeBeforeMtpActivated_ShouldReturnFalse(long blockTime)
        {
            this.subject.BlockTime = DateTimeOffset.FromUnixTimeSeconds(blockTime);

            Assert.False(this.subject.IsMtp());
        }

        [Theory]
        [InlineData(1544443200)]
        [InlineData(1544443201)]
        [InlineData(UInt32.MaxValue)]
        public void IsMtp_WithTimeAfterOrEqualMtpActivated_ShouldReturnTrue(long blockTime)
        {
            this.subject.BlockTime = DateTimeOffset.FromUnixTimeSeconds(blockTime);

            Assert.True(this.subject.IsMtp());
        }

        [Fact]
        public void MtpData_WhenAssigned_ShouldUpdated()
        {
            var value = new MtpData();

            this.subject.SetMtpData(value);

            Assert.Same(value, this.subject.GetMtpData());
        }

        [Fact]
        public void MtpHash_WhenAssigned_ShouldUpdated()
        {
            this.subject.SetMtpHash(uint256.One);

            Assert.Same(uint256.One, this.subject.GetMtpHash());
        }

        [Fact]
        public void MtpVersion_WhenAssigned_ShouldUpdated()
        {
            this.subject.SetMtpVersion(10);

            Assert.Equal(10, this.subject.GetMtpVersion());
        }

        [Fact]
        public void Reserved1_WhenAssigned_ShouldUpdated()
        {
            this.subject.SetReserved1(uint256.One);

            Assert.Same(uint256.One, this.subject.GetReserved1());
        }

        [Fact]
        public void Reserved2_WhenAssigned_ShouldUpdated()
        {
            this.subject.SetReserved2(uint256.One);

            Assert.Same(uint256.One, this.subject.GetReserved2());
        }
    }
}
