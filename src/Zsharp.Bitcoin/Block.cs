namespace Zsharp.Bitcoin
{
    sealed class Block : NBitcoin.Block
    {
        readonly ConsensusFactory consensusFactory;

        #pragma warning disable CS0618
        public Block(ConsensusFactory consensusFactory, BlockHeader header)
            : base(header)
        {
            this.consensusFactory = consensusFactory;
        }
        #pragma warning restore CS0618

        public override NBitcoin.ConsensusFactory GetConsensusFactory() => this.consensusFactory;
    }
}
