namespace Zsharp.LightweightIndexer
{
    using System.Threading;
    using System.Threading.Tasks;
    using AsyncEvent;
    using Microsoft.Extensions.Logging;
    using NBitcoin;
    using Zsharp.ServiceModel;

    public sealed class BlockSynchronizer : BackgroundService, IBlockSynchronizer
    {
        readonly IBlockRetriever retriever;
        readonly BlockListener listener;

        public BlockSynchronizer(
            IServiceExceptionHandler exceptionHandler,
            Network network,
            IBlockRetriever retriever,
            IBlockRepository repository,
            ILogger<BlockSynchronizer>? logger)
            : base(exceptionHandler)
        {
            this.Network = network;
            this.retriever = retriever;
            this.Repository = repository;
            this.Logger = logger;
            this.listener = new BlockListener(this);
        }

        public event AsyncEventHandler<BlockEventArgs>? BlockAdded;

        public event AsyncEventHandler<BlockEventArgs>? BlockRemoving;

        internal ILogger? Logger { get; }

        internal Network Network { get; }

        internal IBlockRepository Repository { get; }

        internal Task RaiseBlockAdded(Block block, int height, CancellationToken cancellationToken = default)
        {
            return this.BlockAdded.InvokeAsync(this, new BlockEventArgs(block, height));
        }

        internal Task RaiseBlockRemovingAsync(Block block, int height, CancellationToken cancellationToken = default)
        {
            return this.BlockRemoving.InvokeAsync(this, new BlockEventArgs(block, height));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var retriever = await this.retriever.StartAsync(this.listener, cancellationToken);

            try
            {
                // Watch for stop event.
                var stop = new TaskCompletionSource<bool>();

                cancellationToken.Register(stop.SetCanceled);

                // Wait either stop event or retriever error.
                var stopped = await Task.WhenAny(retriever, stop.Task);

                // Relay what happened (if retriever error it will throw or if stopping triggered it will cancel).
                await stopped;
            }
            finally
            {
                await this.retriever.StopAsync();
            }
        }
    }
}
