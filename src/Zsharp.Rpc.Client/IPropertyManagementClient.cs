namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using Zsharp.Elysium;

    public interface IPropertyManagementClient : IAsyncDisposable
    {
        Task<NBitcoin.Transaction> CreateManagedAsync(
            BitcoinAddress owner,
            PropertyType type,
            TokenType tokenType,
            Property? previous,
            string category,
            string subcategory,
            string name,
            string url,
            string description,
            CancellationToken cancellationToken = default);

        Task<NBitcoin.Transaction> GrantTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress? to,
            TokenAmount amount,
            string? note,
            CancellationToken cancellationToken = default);

        Task<NBitcoin.Transaction> SendTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress to,
            TokenAmount amount,
            Money? referenceAmount,
            CancellationToken cancellationToken = default);
    }
}
