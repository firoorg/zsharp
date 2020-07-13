namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NBitcoin;
    using NBitcoin.RPC;

    public sealed class FakeRpcClient : RpcClient
    {
        public FakeRpcClient(RpcClientFactory factory, RPCClient client)
            : base(factory, client)
        {
            this.StubbedDispose = new Mock<Action<bool>>();
            this.StubbedDisposeAsyncCore = new Mock<Func<ValueTask>>();
        }

        public new RPCClient Client => base.Client;

        public new RpcClientFactory Factory => base.Factory;

        public Mock<Action<bool>> StubbedDispose { get; }

        public Mock<Func<ValueTask>> StubbedDisposeAsyncCore { get; }

        public new Task PopulateElysiumInformationAsync(Transaction tx, CancellationToken cancellationToken = default)
        {
            return base.PopulateElysiumInformationAsync(tx, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            StubbedDispose.Object(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();
            await this.StubbedDisposeAsyncCore.Object();
        }
    }
}
