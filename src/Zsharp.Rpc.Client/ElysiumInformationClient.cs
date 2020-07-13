namespace Zsharp.Rpc.Client
{
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.DataEncoders;
    using NBitcoin.RPC;
    using Newtonsoft.Json.Linq;
    using Zsharp.Elysium;

    public sealed class ElysiumInformationClient : RpcClient, IElysiumInformationClient
    {
        public ElysiumInformationClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
        }

        public async Task<ElysiumBalance> GetBalanceAsync(
            BitcoinAddress address,
            Property property,
            CancellationToken cancellationToken = default)
        {
            var resp = await this.Client.SendCommandAsync("elysium_getbalance", address.ToString(), property.Id.Value);

            var balance = resp.Result.Value<string>("balance");
            var reserved = resp.Result.Value<string>("reserved");

            return new ElysiumBalance(TokenAmount.Parse(balance), TokenAmount.Parse(reserved));
        }

        public async Task<TokenGrants> GetGrantsAsync(Property property, CancellationToken cancellationToken = default)
        {
            var resp = await this.Client.SendCommandAsync("elysium_getgrants", property.Id.Value);

            var histories = ((JArray)resp.Result["issuances"]).Select(i =>
            {
                var grant = i.Value<string?>("grant");
                var revoke = i.Value<string>("revoke");
                var type = (grant != null) ? TokenGrantType.Grant : TokenGrantType.Revoke;
                var tx = uint256.Parse(i.Value<string>("txid"));
                var amount = TokenAmount.Parse(grant ?? revoke);

                return new TokenGrantHistory(type, tx, amount);
            }).ToList();

            return new TokenGrants(
                new PropertyId(resp.Result.Value<long>("propertyid")),
                resp.Result.Value<string>("name"),
                BitcoinAddress.Create(resp.Result.Value<string>("issuer"), this.Client.Network),
                uint256.Parse(resp.Result.Value<string>("creationtxid")),
                TokenAmount.Parse(resp.Result.Value<string>("totaltokens")),
                histories);
        }

        public async Task<ReadOnlySequence<byte>?> GetPayloadAsync(
            uint256 transaction,
            CancellationToken cancellationToken = default)
        {
            RPCResponse resp;

            try
            {
                resp = await this.Client.SendCommandAsync("elysium_getpayload", transaction.ToString());
            }
            catch (RPCException ex) when (ex.Message == "Not a Elysium Protocol transaction")
            {
                return null;
            }

            var payload = Encoders.Hex.DecodeData(resp.Result.Value<string>("payload"));

            return new ReadOnlySequence<byte>(payload);
        }

        public async Task<ElysiumTransaction?> GetTransactionAsync(
            uint256 hash,
            CancellationToken cancellationToken = default)
        {
            RPCResponse resp;

            try
            {
                resp = await this.Client.SendCommandAsync("elysium_gettransaction", hash.ToString());
            }
            catch (RPCException ex) when (ex.Message == "Not a Elysium Protocol transaction")
            {
                return null;
            }

            var sendingAddress = resp.Result.Value<string?>("sendingaddress");
            var referenceAddress = resp.Result.Value<string?>("referenceaddress");
            var confirmationCount = resp.Result.Value<int>("confirmations");
            ElysiumConfirmation? confirmation = null;

            if (confirmationCount > 0)
            {
                confirmation = new ElysiumConfirmation(
                    resp.Result.Value<int>("block"),
                    uint256.Parse(resp.Result.Value<string>("blockhash")),
                    Utils.UnixTimeToDateTime(resp.Result.Value<long>("blocktime")).LocalDateTime,
                    resp.Result.Value<int>("positioninblock"),
                    confirmationCount,
                    resp.Result.Value<bool>("valid"))
                {
                    InvalidReason = resp.Result.Value<string?>("invalidreason"),
                };
            }

            return new ElysiumTransaction(
                uint256.Parse(resp.Result.Value<string>("txid")),
                resp.Result.Value<int>("type_int"),
                resp.Result.Value<int>("version"),
                resp.Result.Value<string>("type"),
                Money.Parse(resp.Result.Value<string>("fee")),
                resp.Result.Value<bool>("ismine"))
            {
                SendingAddress = string.IsNullOrEmpty(sendingAddress)
                    ? null
                    : BitcoinAddress.Create(sendingAddress, this.Client.Network),
                ReferenceAddress = string.IsNullOrEmpty(referenceAddress)
                    ? null
                    : BitcoinAddress.Create(referenceAddress, this.Client.Network),
                Confirmation = confirmation,
            };
        }

        public async Task<IEnumerable<Property>> ListPropertiesAsync(CancellationToken cancellationToken = default)
        {
            var resp = await this.Client.SendCommandAsync("elysium_listproperties");

            return ((JArray)resp.Result)
                .Select(i => new Property(
                    new PropertyId(i.Value<long>("propertyid")),
                    i.Value<string>("name"),
                    i.Value<string>("category"),
                    i.Value<string>("subcategory"),
                    i.Value<string>("url"),
                    i.Value<string>("data"),
                    i.Value<bool>("divisible") ? TokenType.Divisible : TokenType.Indivisible))
                .ToList();
        }
    }
}
