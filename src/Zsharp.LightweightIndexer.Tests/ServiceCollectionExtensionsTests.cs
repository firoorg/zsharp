namespace Zsharp.LightweightIndexer.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Rpc.Client;
    using Zsharp.ServiceModel;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly Mock<IServiceExceptionHandler> exceptionHandler;
        readonly Mock<IRpcClientFactory> rpc;
        readonly Mock<IBlockRepository> repository;
        readonly Mock<ILogger<BlockSynchronizer>> logger;
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.exceptionHandler = new Mock<IServiceExceptionHandler>();
            this.rpc = new Mock<IRpcClientFactory>();
            this.repository = new Mock<IBlockRepository>();
            this.logger = new Mock<ILogger<BlockSynchronizer>>();
            this.subject = new ServiceCollection();

            this.subject.AddSingleton(this.exceptionHandler.Object);
            this.subject.AddSingleton(Networks.Default.Mainnet);
            this.subject.AddSingleton(this.repository.Object);
            this.subject.AddSingleton(this.logger.Object);
            this.subject.AddSingleton(this.rpc.Object);
        }

        [Fact]
        public void AddLightweightIndexer_WithInvalidOptions_ShouldThrowWhenResolvingRegisteredService()
        {
            // Act.
            this.subject.AddLightweightIndexer(options =>
            {
                options.BlockPublisherAddress = null;
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IBlockSynchronizer>());
        }

        [Fact]
        public void AddLightweightIndexer_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddLightweightIndexer(options =>
            {
                options.BlockPublisherAddress = "tcp://localhost:1234";
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();
            var service = services.GetRequiredService<IHostedService>();
            var synchronizer = services.GetRequiredService<IBlockSynchronizer>();

            Assert.Same(service, synchronizer);
        }
    }
}
