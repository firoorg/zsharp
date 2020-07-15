namespace Zsharp.LightweightIndexer
{
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;

    public interface IBlockListener
    {
        Task DiscardBlocksAsync(int start, CancellationToken cancellationToken = default);

        Task<int> GetStartBlockAsync(CancellationToken cancellationToken = default);

        Task<int> ProcessBlockAsync(Block block, int height, CancellationToken cancellationToken = default);
    }
}
