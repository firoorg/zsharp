using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using static Zsharp.Testing.AsynchronousTesting;

namespace Zsharp.ServiceModel.Tests
{
    public sealed class BackgroundServiceTests : IDisposable
    {
        readonly Mock<IBackgroundServiceExceptionHandler> exceptionHandler;
        readonly FakeBackgroundService subject;

        public BackgroundServiceTests()
        {
            this.exceptionHandler = new Mock<IBackgroundServiceExceptionHandler>();
            this.subject = new FakeBackgroundService(this.exceptionHandler.Object);
        }

        public void Dispose()
        {
            this.subject.DisposeAsync().AsTask().Wait();
        }

        [Fact]
        public async Task Dispose_NotStarted_ShouldSuccess()
        {
            // Act.
            await this.subject.DisposeAsync();
            await this.subject.DisposeAsync();

            // Assert.
            this.subject.StubbedDisposeAsync.Verify(f => f(true), Times.Exactly(2));
        }

        [Fact]
        public async Task Dispose_AlreadyStarted_ShouldInvokeStopAsync()
        {
            // Arrange.
            var cancel = new Mock<Action>();

            this.subject.StubbedExecuteAsync
                .Setup(f => f(It.IsAny<CancellationToken>()))
                .Callback<CancellationToken>(cancellationToken => cancellationToken.Register(cancel.Object));

            await this.subject.StartAsync(CancellationToken.None);

            // Act.
            await this.subject.DisposeAsync();
            await this.subject.DisposeAsync();

            // Assert.
            cancel.Verify(f => f(), Times.Once());
            this.subject.StubbedDisposeAsync.Verify(f => f(true), Times.Exactly(2));
        }

        [Fact]
        public Task StartAsync_NotStarted_ShouldStart() => WithCancellationTokenAsync(async cancellationToken =>
        {
            // Act.
            await this.subject.StartAsync(cancellationToken);
            await this.subject.StopAsync(CancellationToken.None);

            // Assert.
            this.subject.StubbedPreExecuteAsync.Verify(
                f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                Times.Once());

            this.subject.StubbedExecuteAsync.Verify(
                f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                Times.Once());

            this.subject.StubbedPostExecuteAsync.Verify(
                f => f(CancellationToken.None),
                Times.Once());

            this.exceptionHandler.Verify(
                h => h.RunAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                Times.Never());
        });

        [Fact]
        public Task StartAsync_AlreadyStarted_ShouldThrow() => WithCancellationTokenAsync(async cancellationToken =>
        {
            // Act.
            await this.subject.StartAsync(cancellationToken);

            // Assert.
            await Assert.ThrowsAsync<InvalidOperationException>(() => this.subject.StartAsync());
        });

        [Fact]
        public Task StartAsync_WhenPreExecuteAsyncThrow_ShouldInvokeExceptionHandler() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedPreExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask(Task.FromException(new Exception())));

                // Act.
                await this.subject.StartAsync(cancellationToken);
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedPreExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                    Times.Once());

                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.IsAny<CancellationToken>()),
                    Times.Never());

                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(It.IsAny<CancellationToken>()),
                    Times.Never());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(this.subject.GetType(), It.Is<Exception>(ex => ex != null), CancellationToken.None),
                    Times.Once());
            });

        [Fact]
        public Task StartAsync_WhenExecuteAsyncThrow_ShouldInvokeExceptionHandler() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask(Task.FromException(new Exception())));

                // Act.
                await this.subject.StartAsync(cancellationToken);
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                    Times.Once());

                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(CancellationToken.None),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(this.subject.GetType(), It.Is<Exception>(ex => ex != null), CancellationToken.None),
                    Times.Once());
            });

        [Fact]
        public Task StartAsync_WhenPostExecuteAsyncThrow_ShouldInvokeExceptionHandler() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedPostExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask(Task.FromException(new Exception())));

                // Act.
                await this.subject.StartAsync(cancellationToken);
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(CancellationToken.None),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(this.subject.GetType(), It.Is<Exception>(ex => ex != null), CancellationToken.None),
                    Times.Once());
            });

        [Fact]
        public Task StopAsync_NotStarted_ShouldThrow()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => this.subject.StopAsync());
        }

        [Fact]
        public Task StopAsync_BackgroundTaskSucceeded_ShouldSuccess() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.Is<CancellationToken>(t => t != cancellationToken)),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                    Times.Never());
            });

        [Fact]
        public Task StopAsync_BackgroundTaskThrowException_ShouldSuccess() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask(Task.FromException(new Exception())));

                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.Is<CancellationToken>(t => t != cancellationToken)),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(this.subject.GetType(), It.Is<Exception>(ex => ex != null), CancellationToken.None),
                    Times.Once());
            });

        [Fact]
        public Task StopAsync_BackgroundTaskCanceled_ShouldSuccess() => WithCancellationTokenAsync(
            async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns<CancellationToken>(async t =>
                    {
                        await Task.Delay(Timeout.InfiniteTimeSpan, t);
                        t.ThrowIfCancellationRequested();
                    });

                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync(CancellationToken.None);

                // Assert.
                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.Is<CancellationToken>(t => t != cancellationToken)),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.RunAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                    Times.Never());
            });

        [Fact]
        public Task StopAsync_WhenCanceled_ShouldThrow() => WithCancellationTokenAsync(
            async (cancellationToken, cancel) =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask(Task.Delay(Timeout.InfiniteTimeSpan)));

                await this.subject.StartAsync(CancellationToken.None);

                // Act.
                var stop = this.subject.StopAsync(cancellationToken);

                cancel();

                // Assert.
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => stop);
            },
            cancellationSource => cancellationSource.Cancel());
    }
}
