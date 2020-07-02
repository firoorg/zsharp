namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRpcClientFactory : IAsyncDisposable
    {
        ValueTask<IChainInformationClient> CreateChainInformationClientAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IElysiumInformationClient> CreateElysiumInformationClientAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IPropertyManagementClient> CreatePropertyManagementClientAsync(
            CancellationToken cancellationToken = default);

        ValueTask<ITransactionManagementClient> CreateTransactionManagementClientAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IWalletClient> CreateWalletClientAsync(CancellationToken cancellationToken = default);
    }
}
