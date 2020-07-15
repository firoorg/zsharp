namespace Zsharp.LightweightIndexer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBlockRetriever : IAsyncDisposable, IDisposable
    {
        Task<Task> StartAsync(IBlockListener listener, CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
