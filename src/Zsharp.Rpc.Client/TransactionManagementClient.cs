namespace Zsharp.Rpc.Client
{
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;

    public sealed class TransactionManagementClient : RpcClient, ITransactionManagementClient
    {
        public TransactionManagementClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
        }

        public Task<uint256> PublishAsync(
            Transaction transaction,
            bool? disableSafeFee = null,
            CancellationToken cancellationToken = default)
        {
            return this.Client.SendRawTransactionAsync(transaction);
        }
    }
}
