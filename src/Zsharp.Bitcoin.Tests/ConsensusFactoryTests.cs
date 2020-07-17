namespace Zsharp.Bitcoin.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class ConsensusFactoryTests
    {
        readonly ConsensusFactory subject;

        public ConsensusFactoryTests()
        {
            this.subject = Networks.Default.Mainnet.Consensus.ConsensusFactory;
        }

        [Fact]
        public void CreateBlock_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var block = this.subject.CreateBlock();

            Assert.IsNotType<Block>(block);
        }

        [Fact]
        public void CreateBlockHeader_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var header = this.subject.CreateBlockHeader();

            Assert.IsNotType<BlockHeader>(header);
        }

        [Fact]
        public void CreateTransaction_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var transaction = this.subject.CreateTransaction();

            Assert.IsNotType<Transaction>(transaction);
        }

        [Fact]
        public void CreateTxOut_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var output = this.subject.CreateTxOut();

            Assert.IsNotType<TxOut>(output);
        }
    }
}
