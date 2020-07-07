namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;

    public sealed class WalletClient : RpcClient, IWalletClient
    {
        public WalletClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
        }

        public Task<BitcoinAddress> GetNewAddressAsync(CancellationToken cancellationToken = default)
        {
            return this.Client.GetNewAddressAsync();
        }

        public Task<uint256> TransferAsync(
            BitcoinAddress receiver,
            Money amount,
            string? comment,
            string? receiverNote,
            bool? includeFee,
            CancellationToken cancellationToken = default)
        {
            if (amount <= Money.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The value is not valid.");
            }

            return this.Client.SendToAddressAsync(receiver, amount, comment, receiverNote, includeFee ?? false);
        }
    }
}
