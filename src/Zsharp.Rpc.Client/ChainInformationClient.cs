namespace Zsharp.Rpc.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.RPC;

    public sealed class ChainInformationClient : RpcClient, IChainInformationClient
    {
        public ChainInformationClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
        }

        public async Task<Block?> GetBlockAsync(uint256 hash, CancellationToken cancellationToken = default)
        {
            Block? block;

            try
            {
                block = await this.Client.GetBlockAsync(hash);
            }
            catch (RPCException ex) when (ex.RPCCode == RPCErrorCode.RPC_INVALID_ADDRESS_OR_KEY)
            {
                return null;
            }

            foreach (var tx in block.Transactions)
            {
                await this.PopulateElysiumInformationAsync(tx, cancellationToken);
            }

            return block;
        }

        public async Task<Block?> GetBlockAsync(int height, CancellationToken cancellationToken = default)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "The value is not a valid height.");
            }

            Block? block;

            try
            {
                block = await this.Client.GetBlockAsync(height);
            }
            catch (RPCException ex) when (ex.RPCCode == RPCErrorCode.RPC_INVALID_PARAMETER)
            {
                return null;
            }

            foreach (var tx in block.Transactions)
            {
                await this.PopulateElysiumInformationAsync(tx, cancellationToken);
            }

            return block;
        }

        public async Task<BlockHeader?> GetBlockHeaderAsync(uint256 hash, CancellationToken cancellationToken = default)
        {
            BlockHeader? header;

            try
            {
                header = await this.Client.GetBlockHeaderAsync(hash);
            }
            catch (RPCException ex) when (ex.RPCCode == RPCErrorCode.RPC_INVALID_ADDRESS_OR_KEY)
            {
                return null;
            }

            return header;
        }

        public async Task<BlockHeader?> GetBlockHeaderAsync(int height, CancellationToken cancellationToken = default)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "The value is not a valid height.");
            }

            BlockHeader? header;

            try
            {
                header = await this.Client.GetBlockHeaderAsync(height);
            }
            catch (RPCException ex) when (ex.RPCCode == RPCErrorCode.RPC_INVALID_PARAMETER)
            {
                return null;
            }

            return header;
        }

        public Task<BlockchainInfo> GetChainInfoAsync(CancellationToken cancellationToken = default)
        {
            return this.Client.GetBlockchainInfoAsync();
        }

        public async Task<Transaction?> GetTransactionAsync(uint256 hash, CancellationToken cancellationToken = default)
        {
            Transaction? tx;

            try
            {
                tx = await this.Client.GetRawTransactionAsync(hash);
            }
            catch (RPCException ex) when (ex.RPCCode == RPCErrorCode.RPC_INVALID_ADDRESS_OR_KEY)
            {
                return null;
            }

            return tx;
        }
    }
}
