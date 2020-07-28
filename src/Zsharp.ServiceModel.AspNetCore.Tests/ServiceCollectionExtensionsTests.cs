namespace Zsharp.ServiceModel.AspNetCore.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly Mock<ILoggerFactory> logger;
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.logger = new Mock<ILoggerFactory>();
            this.subject = new ServiceCollection();

            this.subject.AddSingleton(this.logger.Object);
        }

        [Fact]
        public void AddServiceExceptionHandler_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddServiceExceptionHandler();

            // Assert.
            using var services = this.subject.BuildServiceProvider();
            var implementation = services.GetRequiredService<ServiceExceptionHandler>();
            var service = services.GetRequiredService<IServiceExceptionHandler>();

            Assert.Same(implementation, service);
        }
    }
}
