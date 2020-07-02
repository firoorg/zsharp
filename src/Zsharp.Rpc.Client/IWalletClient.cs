namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;

    public interface IWalletClient : IAsyncDisposable
    {
        Task<BitcoinAddress> GetNewAddressAsync(CancellationToken cancellationToken = default);

        Task<uint256> TransferAsync(
            BitcoinAddress receiver,
            Money amount,
            string? comment,
            string? receiverNote,
            bool? includeFee,
            CancellationToken cancellationToken = default);
    }
}
