namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using Zsharp.Elysium;

    public interface IPropertyManagementClient : IAsyncDisposable, IDisposable
    {
        Task<string> CreateManagedAsync(
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

        Task<string> GrantTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress? to,
            TokenAmount amount,
            string? note,
            CancellationToken cancellationToken = default);

        Task<string> SendTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress to,
            TokenAmount amount,
            Money? referenceAmount,
            CancellationToken cancellationToken = default);
    }
}
