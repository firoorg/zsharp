namespace Zsharp.ServiceModel.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;

    sealed class FakeBackgroundService : BackgroundService
    {
        public FakeBackgroundService(IServiceExceptionHandler exceptionHandler)
            : base(exceptionHandler)
        {
            this.StubbedDispose = new Mock<Action<bool>>();
            this.StubbedDisposeAsyncCore = new Mock<Func<ValueTask>>();
            this.StubbedExecuteAsync = new Mock<Func<CancellationToken, Task>>();
            this.StubbedPostExecuteAsync = new Mock<Func<CancellationToken, Task>>();
            this.StubbedPreExecuteAsync = new Mock<Func<CancellationToken, Task>>();
        }

        public Mock<Action<bool>> StubbedDispose { get; }

        public Mock<Func<ValueTask>> StubbedDisposeAsyncCore { get; }

        public Mock<Func<CancellationToken, Task>> StubbedExecuteAsync { get; }

        public Mock<Func<CancellationToken, Task>> StubbedPostExecuteAsync { get; }

        public Mock<Func<CancellationToken, Task>> StubbedPreExecuteAsync { get; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.StubbedDispose.Object(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();
            await this.StubbedDisposeAsyncCore.Object();
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedExecuteAsync.Object(cancellationToken);
        }

        protected override Task PostExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedPostExecuteAsync.Object(cancellationToken);
        }

        protected override Task PreExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedPreExecuteAsync.Object(cancellationToken);
        }
    }
}
