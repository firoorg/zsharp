namespace Zsharp.ServiceModel.AspNetCore.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public sealed class ServiceExceptionHandlerTests
    {
        readonly Mock<ILogger<ServiceExceptionLogger>> logger;
        readonly ServiceExceptionHandler subject;

        public ServiceExceptionHandlerTests()
        {
            var factory = new Mock<ILoggerFactory>();

            factory
                .Setup(f => f.CreateLogger(typeof(ServiceExceptionLogger).FullName))
                .Returns(() => this.logger.Object);

            this.logger = new Mock<ILogger<ServiceExceptionLogger>>();
            this.subject = new ServiceExceptionHandler(factory.Object);
        }

        [Fact]
        public async Task HandleExceptionAsync_WithValidArgs_ShouldInvokeLoggerAndCollector()
        {
            // Arrange.
            var service = typeof(BackgroundService);
            var ex = new Exception();

            // Act.
            await this.subject.HandleExceptionAsync(service, ex);

            // Assert.
            var collected = Assert.Single(this.subject.Collector.Exceptions);

            Assert.Equal(ex, collected.Exception);
            Assert.Equal(service, collected.Service);

            this.logger.Verify(
                l => l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((a, t) => true),
                    ex,
                    It.Is<Func<It.IsAnyType, Exception, string>>((a, t) => true)),
                Times.Once());
        }
    }
}
