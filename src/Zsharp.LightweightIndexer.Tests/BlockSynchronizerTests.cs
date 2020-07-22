namespace Zsharp.LightweightIndexer.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AsyncEvent;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NBitcoin;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.ServiceModel;
    using Zsharp.Testing;
    using static Zsharp.Testing.AsynchronousTesting;

    public sealed class BlockSynchronizerTests : IDisposable
    {
        readonly Mock<IServiceExceptionHandler> exceptionHandler;
        readonly TaskCompletionSource<object?> background;
        readonly Task<IBlockListener> listener;
        readonly Mock<IBlockRetriever> retriever;
        readonly Mock<LightweightIndexer.IBlockRepository> repository;
        readonly Mock<ILogger<BlockSynchronizer>> logger;
        readonly Mock<AsyncEventHandler<BlockEventArgs>> blockAdded;
        readonly Mock<AsyncEventHandler<BlockEventArgs>> blockRemoving;
        readonly BlockSynchronizer subject;

        public BlockSynchronizerTests()
        {
            var started = new TaskCompletionSource<IBlockListener>();

            this.exceptionHandler = new Mock<IServiceExceptionHandler>();
            this.background = new TaskCompletionSource<object?>();
            this.listener = started.Task;

            this.retriever = new Mock<IBlockRetriever>();
            this.retriever
                .Setup(r => r.StartAsync(It.IsAny<IBlockListener>(), It.IsAny<CancellationToken>()))
                .Callback((IBlockListener listener, CancellationToken cancellationToken) =>
                {
                    started.SetResult(listener);
                })
                .ReturnsAsync(this.background.Task);

            this.repository = new Mock<LightweightIndexer.IBlockRepository>();
            this.logger = new Mock<ILogger<BlockSynchronizer>>();
            this.blockAdded = new Mock<AsyncEventHandler<BlockEventArgs>>();
            this.blockRemoving = new Mock<AsyncEventHandler<BlockEventArgs>>();
            this.subject = new BlockSynchronizer(
                this.exceptionHandler.Object,
                Networks.Default.Regtest,
                this.retriever.Object,
                this.repository.Object,
                this.logger.Object);

            try
            {
                this.subject.BlockAdded += this.blockAdded.Object;
                this.subject.BlockRemoving += this.blockRemoving.Object;
            }
            catch
            {
                this.subject.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            this.subject.Dispose();
        }

        [Fact]
        public async Task StartAsync_WhenRetrieverError_ShouldThrowFromExecuteAsync()
        {
            // Arrange.
            var stopped = new TaskCompletionSource<object?>();

            this.background.SetException(new Exception());

            this.retriever
                .Setup(r => r.StopAsync(It.IsAny<CancellationToken>()))
                .Callback(() => stopped.SetResult(null));

            // Act.
            await this.subject.StartAsync();
            await stopped.Task;

            // Assert.
            this.retriever.Verify(
                r => r.StartAsync(It.IsNotNull<IBlockListener>(), It.IsNotIn(CancellationToken.None)),
                Times.Once());

            this.retriever.Verify(
                r => r.StopAsync(default),
                Times.Once());

            this.exceptionHandler.Verify(
                h => h.HandleExceptionAsync(
                    this.subject.GetType(),
                    It.IsNotNull<Exception>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task StopAsync_WhenInvoke_ShouldStopRetriever()
        {
            // Arrange.
            await this.subject.StartAsync();

            // Act.
            await this.subject.StopAsync();

            // Assert.
            this.retriever.Verify(
                r => r.StartAsync(It.IsNotNull<IBlockListener>(), It.IsNotIn(CancellationToken.None)),
                Times.Once());

            this.retriever.Verify(
                r => r.StopAsync(default),
                Times.Once());

            this.exceptionHandler.Verify(
                h => h.HandleExceptionAsync(It.IsAny<Type>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public Task StartAsync_DiscardBlocksAsyncTriggered_ShouldRemoveBlockUntilHeightIsTheSameAsArgument()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .SetupSequence(r => r.GetLatestsBlocksAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((new[] { TestBlock.Regtest2 }, 2))
                    .ReturnsAsync((new[] { TestBlock.Regtest1 }, 1))
                    .ReturnsAsync((new[] { TestBlock.Regtest0 }, 0))
                    .ReturnsAsync((Enumerable.Empty<Block>(), -1));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                await listener.DiscardBlocksAsync(0, cancellationToken);

                // Assert.
                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == "Re-organize occurred on node side, starting re-organize at our side."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, CancellationToken.None),
                    Times.Exactly(3));

                this.repository.Verify(
                    r => r.RemoveLastBlockAsync(default),
                    Times.Exactly(3));

                this.blockRemoving.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == TestBlock.Regtest2 && e.Height == 2)),
                    Times.Once());

                this.blockRemoving.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == TestBlock.Regtest1 && e.Height == 1)),
                    Times.Once());

                this.blockRemoving.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == TestBlock.Regtest0 && e.Height == 0)),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Removing block 2:{TestBlock.Regtest2.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Removing block 1:{TestBlock.Regtest1.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Removing block 0:{TestBlock.Regtest0.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncTriggeredWithNoLocalBlocks_ShouldReturnZeroFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Enumerable.Empty<Block>(), -1));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.GetStartBlockAsync(cancellationToken);

                // Assert.
                Assert.Equal(0, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == "No any blocks at our side, starting synchronization from the begining."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_GetStartBlockAsyncTriggeredWithSomeLocalBlocks_ShouldReturnNextHeightThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block = TestBlock.Regtest0;

                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((new[] { block }, 0));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.GetStartBlockAsync(cancellationToken);

                // Assert.
                Assert.Equal(1, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Our latest block is 0:{block.GetHash()}, starting synchronization from block 1."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithNonZeroHeightAndNoLocalBlocks_ShouldReturnZeroFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Enumerable.Empty<Block>(), -1));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.ProcessBlockAsync(TestBlock.Regtest1, 1, cancellationToken);

                // Assert.
                Assert.Equal(0, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithZeroHeightAndNonGenesisBlockAndNoLocalBlocks_ShouldThrowFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Enumerable.Empty<Block>(), -1));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                await Assert.ThrowsAsync<ArgumentException>(
                    "block",
                    () => listener.ProcessBlockAsync(TestBlock.Regtest1, 0, cancellationToken));

                // Assert.
                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithGenesisBlockAndNoLocalBlocks_ShouldStoreItAndReturnOneFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var block = TestBlock.Regtest0;

                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Enumerable.Empty<Block>(), -1));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.ProcessBlockAsync(block, 0, cancellationToken);

                // Assert.
                Assert.Equal(1, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Adding block 0:{block.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.repository.Verify(
                    r => r.AddBlockAsync(block, 0, cancellationToken),
                    Times.Once());

                this.blockAdded.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == block && e.Height == 0)),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithUnexpectedHeight_ShouldReturnExpectedHeightFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((new[] { TestBlock.Regtest0 }, 0));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.ProcessBlockAsync(TestBlock.Regtest2, 2, cancellationToken);

                // Assert.
                Assert.Equal(1, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithUnexpectedBlock_ShouldRemoveLatestLocalThenReturnLocalHeightFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                var current = TestBlock.Regtest0;
                var next = TestBlock.Regtest2;

                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((new[] { current }, 0));

                await this.subject.StartAsync();

                var listener = await this.listener;

                // Act.
                var result = await listener.ProcessBlockAsync(next, 1, cancellationToken);

                // Assert.
                Assert.Equal(0, result);

                this.repository.Verify(
                    r => r.GetLatestsBlocksAsync(1, cancellationToken),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"A new block 1:{next.GetHash()} is depend on {next.Header.HashPrevBlock} but we did not have it."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Removing block 0:{current.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.blockRemoving.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == current && e.Height == 0)),
                    Times.Once());

                this.repository.Verify(
                    r => r.RemoveLastBlockAsync(default),
                    Times.Once());
            });
        }

        [Fact]
        public Task StartAsync_ProcessBlockAsyncTriggeredWithExpectedBlock_ShouldStoreItAndReturnNextHeightFromThere()
        {
            return WithCancellationTokenAsync(async cancellationToken =>
            {
                // Arrange.
                this.repository
                    .Setup(r => r.GetLatestsBlocksAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((new[] { TestBlock.Regtest0 }, 0));

                await this.subject.StartAsync();

                var listener = await this.listener;
                var block = TestBlock.Regtest1;

                // Act.
                var result = await listener.ProcessBlockAsync(block, 1, cancellationToken);

                // Assert.
                Assert.Equal(2, result);

                this.logger.Verify(
                    l => l.Log(
                        LogLevel.Information,
                        0,
                        It.Is<It.IsAnyType>((a, t) => a.ToString() == $"Adding block 1:{block.GetHash()}."),
                        null,
                        It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                    Times.Once());

                this.repository.Verify(
                    r => r.AddBlockAsync(block, 1, cancellationToken),
                    Times.Once());

                this.blockAdded.Verify(
                    f => f(this.subject, It.Is<BlockEventArgs>(e => e.Block == block && e.Height == 1)),
                    Times.Once());
            });
        }
    }
}
