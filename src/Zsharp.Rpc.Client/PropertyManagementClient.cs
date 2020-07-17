namespace Zsharp.Rpc.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;
    using Newtonsoft.Json.Linq;
    using Zsharp.Elysium;

    public sealed class PropertyManagementClient : RpcClient, IPropertyManagementClient
    {
        public PropertyManagementClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
        }

        public async Task<string> CreateManagedAsync(
            BitcoinAddress owner,
            PropertyType type,
            TokenType tokenType,
            Property? previous,
            string category,
            string subcategory,
            string name,
            string url,
            string description,
            CancellationToken cancellationToken = default)
        {
            var resp = await this.Client.SendCommandAsync(
                "elysium_sendissuancemanaged",
                owner.ToString(),
                (int)type,
                (int)tokenType,
                previous != null ? previous.Id.Value : 0,
                category,
                subcategory,
                name,
                url,
                description);

            return resp.Result.Value<string>();
        }

        public async Task<string> GrantTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress? to,
            TokenAmount amount,
            string? note,
            CancellationToken cancellationToken = default)
        {
            if (amount <= TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The value is not valid.");
            }

            // Setup arguments.
            var args = new Collection<object>()
            {
                from.ToString(),
                ReferenceEquals(to, null) ? string.Empty : to.ToString(),
                property.Id.Value,
                amount.ToString(property.TokenType),
            };

            if (note != null)
            {
                args.Add(note);
            }

            // Invoke RPC.
            var resp = await this.Client.SendCommandAsync("elysium_sendgrant", args.ToArray());

            return resp.Result.Value<string>();
        }

        public async Task<string> SendTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress to,
            TokenAmount amount,
            Money? referenceAmount,
            CancellationToken cancellationToken = default)
        {
            if (amount <= TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The value is not valid.");
            }

            // Setup arguments.
            var args = new Collection<object>()
            {
                from.ToString(),
                to.ToString(),
                property.Id.Value,
                amount.ToString(property.TokenType),
            };

            if (referenceAmount != null)
            {
                if (referenceAmount <= Money.Zero)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(referenceAmount),
                        referenceAmount,
                        "The value is not valid.");
                }

                args.Add(string.Empty); // redeemaddress, which Zcoin did not use.
                args.Add(referenceAmount.ToDecimal(MoneyUnit.BTC).ToString());
            }

            // Invoke RPC.
            var resp = await this.Client.SendCommandAsync("elysium_send", args.ToArray());

            return resp.Result.Value<string>();
        }
    }
}
