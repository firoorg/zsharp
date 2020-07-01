namespace Zsharp.Bitcoin.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class BlockTests
    {
        readonly ConsensusFactory factory;
        readonly Block subject;

        public BlockTests()
        {
            this.factory = Networks.Default.Mainnet.Consensus.ConsensusFactory;
            this.subject = this.factory.CreateBlock();
        }

        [Fact]
        public void GetConsensusFactory_WhenInvoke_ShouldReturnZcoinVersion()
        {
            Assert.Same(this.factory, this.subject.GetConsensusFactory());
        }
    }
}
