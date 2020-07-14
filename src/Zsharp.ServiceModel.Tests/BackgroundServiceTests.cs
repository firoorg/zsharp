namespace Zsharp.ServiceModel.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;
    using static Zsharp.Testing.AsynchronousTesting;

    public sealed class BackgroundServiceTests : IDisposable
    {
        readonly Mock<IServiceExceptionHandler> exceptionHandler;
        readonly FakeBackgroundService subject;

        public BackgroundServiceTests()
        {
            this.exceptionHandler = new Mock<IServiceExceptionHandler>();
            this.subject = new FakeBackgroundService(this.exceptionHandler.Object);
        }

        public void Dispose()
        {
            this.subject.Dispose();
        }

        [Fact]
        public void Dispose_NotStarted_ShouldSuccess()
        {
            this.subject.Dispose();

            this.subject.StubbedDispose.Verify(f => f(true), Times.Once());
        }

        [Fact]
        public async Task Dispose_AlreadyStarted_ShouldStop()
        {
            // Arrange.
            var cancel = new Mock<Action>();

            this.subject.StubbedExecuteAsync
                .Setup(f => f(It.IsNotIn(CancellationToken.None)))
                .Callback<CancellationToken>(cancellationToken => cancellationToken.Register(cancel.Object));

            await this.subject.StartAsync();

            // Act.
            this.subject.Dispose();

            // Assert.
            cancel.Verify(f => f(), Times.Once());

            this.subject.StubbedDispose.Verify(f => f(true), Times.Once());
        }

        [Fact]
        public async Task DisposeAsync_NotStarted_ShouldSuccess()
        {
            await this.subject.DisposeAsync();

            this.subject.StubbedDisposeAsyncCore.Verify(f => f(), Times.Once());
            this.subject.StubbedDispose.Verify(f => f(false), Times.Once());
        }

        [Fact]
        public async Task DisposeAsync_AlreadyStarted_ShouldStop()
        {
            // Arrange.
            var cancel = new Mock<Action>();

            this.subject.StubbedExecuteAsync
                .Setup(f => f(It.IsNotIn(CancellationToken.None)))
                .Callback<CancellationToken>(cancellationToken => cancellationToken.Register(cancel.Object));

            await this.subject.StartAsync();

            // Act.
            await this.subject.DisposeAsync();

            // Assert.
            cancel.Verify(f => f(), Times.Once());

            this.subject.StubbedDisposeAsyncCore.Verify(f => f(), Times.Once());
            this.subject.StubbedDispose.Verify(f => f(false), Times.Once());
        }

        [Fact]
        public Task StartAsync_NotStarted_ShouldStart() => WithCancellationTokenAsync(async cancellationToken =>
        {
            // Arrange.
            using (var started = new ManualResetEventSlim())
            {
                this.subject.StubbedPostExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Callback(() => started.Set());

                // Act.
                await this.subject.StartAsync(cancellationToken);

                started.Wait();

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
            }
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
        public Task StartAsync_WhenPreExecuteAsyncThrow_ShouldInvokeExceptionHandler()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                using (var started = new ManualResetEventSlim())
                {
                    this.subject.StubbedPreExecuteAsync
                        .Setup(f => f(It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception());

                    this.exceptionHandler
                        .Setup(
                            h => h.HandleExceptionAsync(
                                It.IsAny<Type>(),
                                It.IsAny<Exception>(),
                                It.IsAny<CancellationToken>()))
                        .Callback(() => started.Set());

                    // Act.
                    await this.subject.StartAsync(cancellationToken);

                    started.Wait();

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
                        h => h.HandleExceptionAsync(this.subject.GetType(), It.IsAny<Exception>(), default),
                        Times.Once());
                }
            });
        }

        [Fact]
        public Task StartAsync_WhenExecuteAsyncThrow_ShouldInvokeExceptionHandler()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                using (var started = new ManualResetEventSlim())
                {
                    this.subject.StubbedExecuteAsync
                        .Setup(f => f(It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception());

                    this.exceptionHandler
                        .Setup(
                            h => h.HandleExceptionAsync(
                                It.IsAny<Type>(),
                                It.IsAny<Exception>(),
                                It.IsAny<CancellationToken>()))
                        .Callback(() => started.Set());

                    // Act.
                    await this.subject.StartAsync(cancellationToken);

                    started.Wait();

                    // Assert.
                    this.subject.StubbedPreExecuteAsync.Verify(
                        f => f(It.IsNotIn(cancellationToken, default)),
                        Times.Once());

                    this.subject.StubbedExecuteAsync.Verify(
                        f => f(It.IsNotIn(cancellationToken, default)),
                        Times.Once());

                    this.subject.StubbedPostExecuteAsync.Verify(
                        f => f(default),
                        Times.Once());

                    this.exceptionHandler.Verify(
                        h => h.HandleExceptionAsync(this.subject.GetType(), It.IsAny<Exception>(), default),
                        Times.Once());
                }
            });
        }

        [Fact]
        public Task StartAsync_WhenPostExecuteAsyncThrow_ShouldInvokeExceptionHandler()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                using (var started = new ManualResetEventSlim())
                {
                    this.subject.StubbedPostExecuteAsync
                        .Setup(f => f(It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception());

                    this.exceptionHandler
                        .Setup(
                            h => h.HandleExceptionAsync(
                                It.IsAny<Type>(),
                                It.IsAny<Exception>(),
                                It.IsAny<CancellationToken>()))
                        .Callback(() => started.Set());

                    // Act.
                    await this.subject.StartAsync(cancellationToken);

                    started.Wait();

                    // Assert.
                    this.subject.StubbedPreExecuteAsync.Verify(
                        f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                        Times.Once());

                    this.subject.StubbedExecuteAsync.Verify(
                        f => f(It.IsNotIn(cancellationToken, CancellationToken.None)),
                        Times.Once());

                    this.subject.StubbedPostExecuteAsync.Verify(
                        f => f(default),
                        Times.Once());

                    this.exceptionHandler.Verify(
                        h => h.HandleExceptionAsync(this.subject.GetType(), It.IsAny<Exception>(), default),
                        Times.Once());
                }
            });
        }

        [Fact]
        public Task StopAsync_NotStarted_ShouldThrow()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => this.subject.StopAsync());
        }

        [Fact]
        public Task StopAsync_BackgroundTaskSucceeded_ShouldNotThrow()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync();

                // Assert.
                this.subject.StubbedPreExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(default),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.HandleExceptionAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                    Times.Never());
            });
        }

        [Fact]
        public Task StopAsync_BackgroundTaskThrowException_ShouldNotThrow()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception());

                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync();

                // Assert.
                this.subject.StubbedPreExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(default),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.HandleExceptionAsync(this.subject.GetType(), It.IsAny<Exception>(), default),
                    Times.Once());
            });
        }

        [Fact]
        public Task StopAsync_BackgroundTaskCanceled_ShouldNotThrow()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.subject.StubbedExecuteAsync
                    .Setup(f => f(It.IsAny<CancellationToken>()))
                    .Returns<CancellationToken>(async cancellationToken =>
                    {
                        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                    });

                await this.subject.StartAsync(cancellationToken);

                // Act.
                await this.subject.StopAsync();

                // Assert.
                this.subject.StubbedPreExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedExecuteAsync.Verify(
                    f => f(It.IsNotIn(cancellationToken, default)),
                    Times.Once());

                this.subject.StubbedPostExecuteAsync.Verify(
                    f => f(default),
                    Times.Once());

                this.exceptionHandler.Verify(
                    h => h.HandleExceptionAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                    Times.Never());
            });
        }

        [Fact]
        public Task StopAsync_WhenCanceled_ShouldThrow()
        {
            return WithCancellationTokenAsync(
                async (cancellationToken, cancel) =>
                {
                    // Arrange.
                    this.subject.StubbedExecuteAsync
                        .Setup(f => f(It.IsAny<CancellationToken>()))
                        .Returns(Task.Delay(Timeout.InfiniteTimeSpan));

                    await this.subject.StartAsync();

                    // Act.
                    var stop = this.subject.StopAsync(cancellationToken);

                    cancel();

                    // Assert.
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => stop);
                },
                canceler => canceler.Cancel());
        }
    }
}
