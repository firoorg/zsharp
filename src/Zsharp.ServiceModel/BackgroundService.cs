namespace Zsharp.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public abstract class BackgroundService : IAsyncDisposable, IHostedService
    {
        readonly IBackgroundServiceExceptionHandler exceptionHandler;
        readonly CancellationTokenSource cancellation;
        Task? background;
        bool disposed;

        protected BackgroundService(IBackgroundServiceExceptionHandler exceptionHandler)
        {
            this.exceptionHandler = exceptionHandler;
            this.cancellation = new CancellationTokenSource();
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (this.background != null)
            {
                throw new InvalidOperationException("The service is already started.");
            }

            this.background = this.RunBackgroundTaskAsync(this.cancellation.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this.background == null)
            {
                throw new InvalidOperationException("The service was not started.");
            }

            this.cancellation.Cancel();

            try
            {
                // This method must ignores any errors that was raised from the background task due to it is already
                // handled by the exception handler. But we still needs to throw OperationCanceledException if
                // cancellationToken is triggered.
                var completed = await Task.WhenAny(
                    this.background,
                    Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));

                if (!ReferenceEquals(completed, this.background))
                {
                    await completed;
                }
            }
            finally
            {
                this.background = null;
            }
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.background != null)
                {
                    await this.StopAsync();
                }

                this.cancellation.Dispose();
            }

            this.disposed = true;
        }

        protected abstract ValueTask ExecuteAsync(CancellationToken cancellationToken = default);

        protected virtual ValueTask PostExecuteAsync(CancellationToken cancellationToken = default) => default;

        protected virtual ValueTask PreExecuteAsync(CancellationToken cancellationToken = default) => default;

        async Task RunBackgroundTaskAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield(); // We don't want the code after this line to run synchronously.

            try
            {
                await this.PreExecuteAsync(cancellationToken);

                try
                {
                    await this.ExecuteAsync(cancellationToken);
                }
                finally
                {
                    await this.PostExecuteAsync();
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException == false)
            {
                await this.exceptionHandler.RunAsync(this.GetType(), ex);
            }
        }
    }
}
