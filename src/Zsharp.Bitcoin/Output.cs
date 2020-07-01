namespace Zsharp.Bitcoin
{
    using NBitcoin;

    sealed class Output : TxOut
    {
        readonly ConsensusFactory consensusFactory;

        public Output(ConsensusFactory consensusFactory)
        {
            this.consensusFactory = consensusFactory;
        }

        public override NBitcoin.ConsensusFactory GetConsensusFactory() => this.consensusFactory;
    }
}
