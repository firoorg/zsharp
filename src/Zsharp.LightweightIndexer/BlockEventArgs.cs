namespace Zsharp.LightweightIndexer
{
    using System;
    using NBitcoin;

    public class BlockEventArgs
    {
        public BlockEventArgs(Block block, int height)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            this.Block = block;
            this.Height = height;
        }

        public Block Block { get; }

        public int Height { get; }
    }
}
