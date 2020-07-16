namespace Zsharp.LightweightIndexer
{
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;

    public interface IBlockRepository
    {
        ValueTask AddAsync(Block block, int height, CancellationToken cancellationToken = default);

        ValueTask<(Block? Block, int Height)> GetLastAsync(CancellationToken cancellationToken = default);

        ValueTask RemoveLastAsync(CancellationToken cancellationToken = default);
    }
}
