namespace Zsharp.Threading.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using static Zsharp.Testing.AsynchronousTesting;

    public sealed class CancellationTokenExtenionsTests : IDisposable
    {
        readonly CancellationTokenSource source;

        public CancellationTokenExtenionsTests()
        {
            this.source = new CancellationTokenSource();
        }

        public void Dispose()
        {
            this.source.Dispose();
        }

        [Fact]
        public async Task WaitAsync_OnNonCancelable_ShouldThrow()
        {
            var subject = CancellationToken.None;

            await Assert.ThrowsAsync<InvalidOperationException>(() => subject.WaitAsync());
        }

        [Fact]
        public async Task WaitAsync_WithUncancelableCancellationToken_ShouldCompletedWhenCanceled()
        {
            // Act.
            var task = this.source.Token.WaitAsync();

            this.source.Cancel();

            // Assert.
            await task;
        }

        [Fact]
        public Task WaitAsync_WithCancellationToken_ShouldCompletedWhenCanceled()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Act.
                var task = this.source.Token.WaitAsync(cancellationToken);

                this.source.Cancel();

                // Assert.
                await task;
            });
        }

        [Fact]
        public Task WaitAsync_WithCancellationToken_ShouldCancelWhenCancellationTokenCanceled()
        {
            return WithCancellationTokenAsync(
                async (cancellationToken, cancel) =>
                {
                    // Act.
                    var task = this.source.Token.WaitAsync(cancellationToken);

                    cancel();

                    // Assert.
                    await Assert.ThrowsAsync<TaskCanceledException>(() => task);
                },
                source => source.Cancel());
        }
    }
}
