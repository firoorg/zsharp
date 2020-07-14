namespace Zsharp.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public abstract class BackgroundService : IAsyncDisposable, IDisposable, IHostedService
    {
        readonly IServiceExceptionHandler exceptionHandler;
        readonly CancellationTokenSource canceler;
        Task? background;
        bool disposed;

        protected BackgroundService(IServiceExceptionHandler exceptionHandler)
        {
            this.exceptionHandler = exceptionHandler;
            this.canceler = new CancellationTokenSource();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsyncCore();
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (this.background != null)
            {
                throw new InvalidOperationException("The service is already started.");
            }

            this.background = this.RunBackgroundTaskAsync(this.canceler.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this.background == null)
            {
                throw new InvalidOperationException("The service was not started.");
            }

            var canceled = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            this.canceler.Cancel();

            try
            {
                // This method must ignores any errors that was raised from the background task due to it is already
                // handled by the exception handler. But we still needs to throw OperationCanceledException if
                // cancellationToken is triggered.
                var completed = await Task.WhenAny(this.background, canceled);

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

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.background != null)
                {
                    this.StopAsync().Wait();
                }

                this.canceler.Dispose();
            }

            this.disposed = true;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (this.background != null)
            {
                await this.StopAsync();
            }

            this.canceler.Dispose();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

        protected virtual Task PostExecuteAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        protected virtual Task PreExecuteAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

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
                await this.exceptionHandler.HandleExceptionAsync(this.GetType(), ex);
            }
        }
    }
}
