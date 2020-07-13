namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;

    public interface IChainInformationClient : IAsyncDisposable, IDisposable
    {
        Task<Block?> GetBlockAsync(uint256 hash, CancellationToken cancellationToken = default);

        Task<Block?> GetBlockAsync(int height, CancellationToken cancellationToken = default);

        Task<BlockHeader?> GetBlockHeaderAsync(uint256 hash, CancellationToken cancellationToken = default);

        Task<BlockHeader?> GetBlockHeaderAsync(int height, CancellationToken cancellationToken = default);

        Task<BlockchainInfo> GetChainInfoAsync(CancellationToken cancellationToken = default);

        Task<Transaction?> GetTransactionAsync(uint256 hash, CancellationToken cancellationToken = default);
    }
}
