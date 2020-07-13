namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;

    public interface ITransactionManagementClient : IAsyncDisposable, IDisposable
    {
        Task<uint256> PublishAsync(
            Transaction transaction,
            bool? disableSafeFee = null,
            CancellationToken cancellationToken = default);
    }
}
