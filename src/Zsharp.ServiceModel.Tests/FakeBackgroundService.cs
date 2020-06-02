using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Zsharp.ServiceModel.Tests
{
    public sealed class FakeBackgroundService : BackgroundService
    {
        public FakeBackgroundService(IBackgroundServiceExceptionHandler exceptionHandler) : base(exceptionHandler)
        {
            StubbedDisposeAsync = new Mock<Func<bool, ValueTask>>();
            StubbedExecuteAsync = new Mock<Func<CancellationToken, ValueTask>>();
            StubbedPostExecuteAsync = new Mock<Func<CancellationToken, ValueTask>>();
            StubbedPreExecuteAsync = new Mock<Func<CancellationToken, ValueTask>>();
        }

        public Mock<Func<bool, ValueTask>> StubbedDisposeAsync { get; }

        public Mock<Func<CancellationToken, ValueTask>> StubbedExecuteAsync { get; }

        public Mock<Func<CancellationToken, ValueTask>> StubbedPostExecuteAsync { get; }

        public Mock<Func<CancellationToken, ValueTask>> StubbedPreExecuteAsync { get; }

        protected override async ValueTask DisposeAsync(bool disposing)
        {
            await base.DisposeAsync(disposing);
            await StubbedDisposeAsync.Object(disposing);
        }

        protected override ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedExecuteAsync.Object(cancellationToken);
        }

        protected override ValueTask PostExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedPostExecuteAsync.Object(cancellationToken);
        }

        protected override ValueTask PreExecuteAsync(CancellationToken cancellationToken = default)
        {
            return StubbedPreExecuteAsync.Object(cancellationToken);
        }
    }
}
