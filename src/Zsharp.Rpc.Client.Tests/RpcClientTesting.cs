namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;
    using NBitcoin.Tests;
    using Zsharp.Elysium;
    using Zsharp.Elysium.TransactionSerializers;
    using Zsharp.Testing;

    public abstract class RpcClientTesting<T> : IDisposable where T : IDisposable
    {
        static IEnumerable<ITransactionPayloadSerializer> ElysiumSerializers = new ITransactionPayloadSerializer[]
        {
            new CreateManagedPropertySerializer(),
            new GrantTokensSerializer(),
            new SimpleSendSerializer()
        };

        readonly NodeBuilder nodeBuilder;
        readonly Lazy<T> subject;
        bool disposed;

        protected RpcClientTesting(NodeConfigParameters? config = null)
        {
            this.nodeBuilder = NodeBuilderFactory.CreateNodeBuilder(GetType());

            try
            {
                this.nodeBuilder.ConfigParameters.Add("dandelion", "0");
                this.nodeBuilder.ConfigParameters.Add("elysium", "1");

                if (config != null)
                {
                    this.nodeBuilder.ConfigParameters.Import(config, true);
                }

                this.Node = this.nodeBuilder.CreateNode(true);
                this.Client = this.Node.CreateRPCClient();
                this.Factory = new RpcClientFactory(
                    this.nodeBuilder.Network,
                    this.Node.RPCUri,
                    RPCCredentialString.Parse(this.Node.GetRPCAuth()),
                    new TransactionSerializer(ElysiumSerializers));

                this.subject = new Lazy<T>(CreateSubject);
            }
            catch
            {
                this.nodeBuilder.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected RPCClient Client { get; }

        protected RpcClientFactory Factory { get; }

        protected Network Network => this.nodeBuilder.Network;

        protected CoreNode Node { get; }

        protected T Subject => this.subject.Value;

        protected abstract T CreateSubject();

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.subject.IsValueCreated)
                {
                    this.subject.Value.Dispose();
                }

                this.nodeBuilder.Dispose();
            }

            this.disposed = true;
        }

        protected async Task<uint256> FundAddressAsync(BitcoinAddress address, Money amount)
        {
            await using (var wallet = await this.Factory.CreateWalletClientAsync())
            {
                return await wallet.TransferAsync(address, amount, null, null, false);
            }
        }

        protected async Task<BitcoinAddress> GenerateNewAddressAsync()
        {
            await using (var client = await this.Factory.CreateWalletClientAsync())
            {
                return await client.GetNewAddressAsync();
            }
        }

        protected async Task<Block> GetBlockAsync(uint256 hash)
        {
            await using (var client = await Factory.CreateChainInformationClientAsync())
            {
                var block = await client.GetBlockAsync(hash);

                if (block == null)
                {
                    throw new ArgumentException("The value is not a valid block's hash.", nameof(hash));
                }

                return block;
            }
        }

        protected async Task<Property> GetPropertyAsync(string name)
        {
            await using (var client = await this.Factory.CreateElysiumInformationClientAsync())
            {
                var props = await client.ListPropertiesAsync();

                return props.Single(p => p.Name == name);
            }
        }

        protected async Task<IEnumerable<NBitcoin.Transaction>> GetTransactionsAsync(uint256 block)
        {
            await using (var client = await this.Factory.CreateChainInformationClientAsync())
            {
                var result = await client.GetBlockAsync(block);

                if (result == null)
                {
                    throw new ArgumentException("The value is not a valid block hash.", nameof(block));
                }

                return result.Transactions;
            }
        }

        protected async Task<string> GrantTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress? to,
            TokenAmount amount)
        {
            await using (var client = await this.Factory.CreatePropertyManagementClientAsync())
            {
                return await client.GrantTokensAsync(property, from, to, amount, null);
            }
        }

        protected async Task<uint256> PublishTransactionAsync(NBitcoin.Transaction tx)
        {
            await using (var client = await this.Factory.CreateTransactionManagementClientAsync())
            {
                return await client.PublishAsync(tx, true);
            }
        }

        protected async Task<string> SendTokensAsync(
            Property property,
            BitcoinAddress from,
            BitcoinAddress to,
            TokenAmount amount)
        {
            await using (var client = await this.Factory.CreatePropertyManagementClientAsync())
            {
                return await client.SendTokensAsync(property, from, to, amount, null);
            }
        }
    }
}
