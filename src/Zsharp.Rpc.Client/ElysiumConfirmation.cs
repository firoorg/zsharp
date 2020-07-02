namespace Zsharp.Rpc.Client
{
    using System;
    using NBitcoin;

    public sealed class ElysiumConfirmation
    {
        public ElysiumConfirmation(int block, uint256 hash, DateTime time, int index, int count, bool valid)
        {
            if (block < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(block));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.Block = block;
            this.BlockHash = hash;
            this.BlockTime = time;
            this.BlockIndex = index;
            this.Count = count;
            this.Valid = valid;
        }

        public int Block { get; }

        public uint256 BlockHash { get; }

        public int BlockIndex { get; }

        public DateTime BlockTime { get; }

        public int Count { get; }

        public string? InvalidReason { get; set; }

        public bool Valid { get; }
    }
}
