namespace Zsharp.Bitcoin.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class OutputTests
    {
        readonly ConsensusFactory factory;
        readonly TxOut subject;

        public OutputTests()
        {
            this.factory = Networks.Default.Mainnet.Consensus.ConsensusFactory;
            this.subject = this.factory.CreateTxOut();
        }

        [Fact]
        public void GetConsensusFactory_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var result = this.subject.GetConsensusFactory();

            Assert.Same(this.factory, result);
        }
    }
}
