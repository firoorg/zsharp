namespace Zsharp.Rpc.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using Zsharp.Elysium;

    public interface IElysiumInformationClient : IAsyncDisposable
    {
        Task<ElysiumBalance?> GetBalanceAsync(
            BitcoinAddress address,
            Property property,
            CancellationToken cancellationToken = default);

        Task<TokenGrants?> GetGrantsAsync(Property property, CancellationToken cancellationToken = default);

        Task<ReadOnlyMemory<byte>?> GetPayloadAsync(uint256 transaction, CancellationToken cancellationToken = default);

        Task<ElysiumTransaction?> GetTransactionAsync(uint256 hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<Property>> ListPropertiesAsync(CancellationToken cancellationToken = default);
    }
}
