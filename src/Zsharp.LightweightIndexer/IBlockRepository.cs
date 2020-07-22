namespace Zsharp.LightweightIndexer
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;

    public interface IBlockRepository
    {
        ValueTask AddBlockAsync(Block block, int height, CancellationToken cancellationToken = default);

        ValueTask<(Block? Block, int Height)> GetBlockAsync(
            uint256 hash,
            CancellationToken cancellationToken = default);

        ValueTask<Block?> GetBlockAsync(int height, CancellationToken cancellationToken = default);

        ValueTask<(IEnumerable<Block> Blocks, int Highest)> GetLatestsBlocksAsync(
            int count,
            CancellationToken cancellationToken = default);

        ValueTask<Transaction?> GetTransactionAsync(uint256 hash, CancellationToken cancellationToken = default);

        ValueTask RemoveLastBlockAsync(CancellationToken cancellationToken = default);
    }
}
