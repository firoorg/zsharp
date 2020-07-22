namespace Zsharp.LightweightIndexer.Entity
{
    using System.Collections.Generic;
    using NBitcoin;
    using Zsharp.Zcoin;

    public sealed class Transaction
    {
        public Transaction(uint256 hash, TransactionType type, short version, int lockTime)
        {
            this.Hash = hash;
            this.Type = type;
            this.Version = version;
            this.LockTime = lockTime;
            this.Blocks = new SortedSet<BlockTransaction>();
            this.Inputs = new SortedSet<Input>();
            this.Outputs = new SortedSet<Output>();
        }

        public SortedSet<BlockTransaction> Blocks { get; set; }

        public ElysiumTransaction? Elysium { get; set; }

        public byte[]? ExtraPayload { get; set; }

        public uint256 Hash { get; }

        public SortedSet<Input> Inputs { get; set; }

        public int LockTime { get; }

        public SortedSet<Output> Outputs { get; set; }

        public TransactionType Type { get; }

        public short Version { get; }
    }
}
