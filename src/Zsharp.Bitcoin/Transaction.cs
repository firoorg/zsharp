namespace Zsharp.Bitcoin
{
    using System.Linq;

    sealed class Transaction : NBitcoin.Transaction
    {
        readonly ConsensusFactory consensusFactory;

        public Transaction(ConsensusFactory consensusFactory)
        {
            this.consensusFactory = consensusFactory;
        }

        public Elysium.Transaction? ElysiumTransaction { get; set; }

        public override bool IsCoinBase
        {
            get
            {
                if (this.IsZerocoinSpend || this.IsSigmaSpend || this.IsZerocoinRemint)
                {
                    return false;
                }

                return base.IsCoinBase;
            }
        }

        public bool IsSigmaSpend => this.Inputs.All(input => input.IsSigmaSpend());

        public bool IsZerocoinRemint => this.Inputs.All(input => input.IsZerocoinRemint());

        public bool IsZerocoinSpend => this.Inputs.All(input => input.IsZerocoinSpend());

        public override NBitcoin.ConsensusFactory GetConsensusFactory() => this.consensusFactory;
    }
}
