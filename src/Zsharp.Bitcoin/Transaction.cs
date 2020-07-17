namespace Zsharp.Bitcoin
{
    using System;
    using System.Linq;
    using NBitcoin;
    using Zsharp.Zcoin;

    sealed class Transaction : NBitcoin.Transaction
    {
        readonly ConsensusFactory consensusFactory;
        byte[] extraPayload;

        public Transaction(ConsensusFactory consensusFactory)
        {
            this.consensusFactory = consensusFactory;
            this.extraPayload = new byte[0];
        }

        public Elysium.Transaction? ElysiumTransaction { get; set; }

        public byte[] ExtraPayload
        {
            get => this.extraPayload;
            set => this.extraPayload = value;
        }

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

        public TransactionType TransactionType { get; set; }

        public override NBitcoin.ConsensusFactory GetConsensusFactory() => this.consensusFactory;

        public override void ReadWrite(BitcoinStream stream)
        {
            if (stream.Serializing)
            {
                // Write.
                var version = this.Version;

                if (version > ushort.MaxValue)
                {
                    throw new InvalidOperationException("Invalid version.");
                }

                this.Version |= (uint)(Convert.ToInt16(this.TransactionType) << 16);

                try
                {
                    base.ReadWrite(stream);
                }
                finally
                {
                    this.Version = version;
                }
            }
            else
            {
                // Read.
                base.ReadWrite(stream);

                this.TransactionType = (TransactionType)Enum.ToObject(typeof(TransactionType), this.Version >> 16);
                this.Version &= 0xFFFF;
            }

            if (this.Version == 3 && this.TransactionType != TransactionType.Normal)
            {
                stream.ReadWriteAsVarString(ref this.extraPayload);
            }
        }
    }
}
