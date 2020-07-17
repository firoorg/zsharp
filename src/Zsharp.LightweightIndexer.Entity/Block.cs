namespace Zsharp.LightweightIndexer.Entity
{
    using System;
    using System.Collections.Generic;
    using NBitcoin;

    public sealed class Block : IComparable<Block?>
    {
        public Block(int height, uint256 hash, DateTime time, int version, Target target, int nonce, uint256 merkleRoot)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            this.Height = height;
            this.Hash = hash;
            this.Time = time;
            this.Version = version;
            this.Target = target;
            this.Nonce = nonce;
            this.MerkleRoot = merkleRoot;
            this.Transactions = new SortedSet<BlockTransaction>();
        }

        public uint256 Hash { get; }

        public int Height { get; }

        public uint256 MerkleRoot { get; }

        public MtpData? MtpData { get; set; }

        public int Nonce { get; }

        public Target Target { get; }

        public DateTime Time { get; }

        public SortedSet<BlockTransaction> Transactions { get; set; }

        public int Version { get; }

        public int CompareTo(Block? other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.Height - other.Height;
        }

        public override bool Equals(object? other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.CompareTo((Block)other) == 0;
        }

        public override int GetHashCode()
        {
            return this.Height.GetHashCode();
        }
    }
}
