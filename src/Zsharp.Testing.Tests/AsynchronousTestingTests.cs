using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Subject=Zsharp.Testing.AsynchronousTesting;

namespace Zsharp.Testing.Tests
{
    public sealed class AsynchronousTestingTests
    {
        [Fact]
        public void WithCancellationToken_WithNonNullTest_ShouldInvokeIt()
        {
            var test = new Mock<Action<CancellationToken>>();

            Subject.WithCancellationToken(test.Object);

            test.Verify(f => f(It.IsNotIn(CancellationToken.None)), Times.Once());
        }

        [Fact]
        public async Task WithCancellationTokenAsync_WithNonNullTest_ShouldInvokeIt()
        {
            var test1 = new Mock<Func<CancellationToken, ValueTask>>();
            var test2 = new Mock<Func<CancellationToken, Action, ValueTask>>();
            var test3 = new Mock<Func<CancellationToken, Func<ValueTask>, ValueTask>>();

            await Subject.WithCancellationTokenAsync(test1.Object);
            await Subject.WithCancellationTokenAsync(test2.Object, cancellationToken => {});
            await Subject.WithCancellationTokenAsync(test3.Object, cancellationToken => default);

            test1.Verify(f => f(It.IsNotIn(CancellationToken.None)), Times.Once());
            test2.Verify(f => f(It.IsNotIn(CancellationToken.None), It.IsNotNull<Action>()), Times.Once());
            test3.Verify(f => f(It.IsNotIn(CancellationToken.None), It.IsNotNull<Func<ValueTask>>()), Times.Once());
        }

        [Fact]
        public async Task WithCancellationTokenAsync_WithNonNullCancel_ShouldInvokeTestWithCancelFunction()
        {
            await Subject.WithCancellationTokenAsync(
                (cancellationToken, cancel) =>
                {
                    Assert.False(cancellationToken.IsCancellationRequested);
                    cancel();
                    Assert.True(cancellationToken.IsCancellationRequested);

                    return default;
                },
                cancellationToken => cancellationToken.Cancel());

            await Subject.WithCancellationTokenAsync(
                async (cancellationToken, cancel) =>
                {
                    Assert.False(cancellationToken.IsCancellationRequested);
                    await cancel();
                    Assert.True(cancellationToken.IsCancellationRequested);
                },
                cancellationToken =>
                {
                    cancellationToken.Cancel();
                    return default;
                });
        }
    }
}
