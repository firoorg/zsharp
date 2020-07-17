namespace Zsharp.LightweightIndexer.Entity
{
    using System.Collections.Generic;
    using NBitcoin;

    public sealed class Transaction
    {
        public Transaction(uint256 hash, int version, int lockTime)
        {
            this.Hash = hash;
            this.Version = version;
            this.LockTime = lockTime;
            this.Blocks = new SortedSet<BlockTransaction>();
            this.Inputs = new SortedSet<Input>();
            this.Outputs = new SortedSet<Output>();
        }

        public SortedSet<BlockTransaction> Blocks { get; set; }

        public ElysiumTransaction? Elysium { get; set; }

        public byte[]? Extra { get; set; }

        public uint256 Hash { get; }

        public SortedSet<Input> Inputs { get; set; }

        public int LockTime { get; }

        public SortedSet<Output> Outputs { get; set; }

        public int Version { get; }
    }
}
