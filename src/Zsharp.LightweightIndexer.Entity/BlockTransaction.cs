namespace Zsharp.LightweightIndexer.Entity
{
    using System;
    using NBitcoin;

    public sealed class BlockTransaction : IComparable<BlockTransaction?>
    {
        public BlockTransaction(uint256 blockHash, int index, uint256 transactionHash)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.BlockHash = blockHash;
            this.Index = index;
            this.TransactionHash = transactionHash;
        }

        public Block? Block { get; set; }

        public uint256 BlockHash { get; }

        public int Index { get; }

        public Transaction? Transaction { get; set; }

        public uint256 TransactionHash { get; }

        public int CompareTo(BlockTransaction? other)
        {
            if (other == null)
            {
                return 1;
            }

            if (this.BlockHash != other.BlockHash)
            {
                return this.BlockHash.CompareTo(other.BlockHash);
            }

            return this.Index - other.Index;
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.CompareTo((BlockTransaction)other) == 0;
        }

        public override int GetHashCode()
        {
            return this.BlockHash.GetHashCode() ^ this.Index.GetHashCode();
        }
    }
}
