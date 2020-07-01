namespace Zsharp.Bitcoin
{
    using NBitcoin;
    using Zsharp.Zcoin;

    sealed class Input : TxIn
    {
        readonly ConsensusFactory consensusFactory;

        public Input(ConsensusFactory consensusFactory)
        {
            this.consensusFactory = consensusFactory;
        }

        public bool IsSigmaSpend =>
            this.prevout.Hash == uint256.Zero &&
            this.prevout.N >= 1 &&
            this.ScriptStartsWith(OperationCode.SigmaSpend);

        public bool IsZerocoinRemint => this.prevout.IsNull && this.ScriptStartsWith(OperationCode.ZerocoinRemint);

        public bool IsZerocoinSpend => this.prevout.IsNull && this.ScriptStartsWith(OperationCode.ZerocoinSpend);

        public override NBitcoin.ConsensusFactory GetConsensusFactory() => this.consensusFactory;

        bool ScriptStartsWith(OperationCode code) =>
            this.scriptSig.Length > 0 &&
            this.scriptSig.ToBytes(true)[0] == (byte)code;
    }
}
