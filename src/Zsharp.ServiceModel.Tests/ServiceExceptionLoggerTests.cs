namespace Zsharp.ServiceModel.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public sealed class ServiceExceptionLoggerTests
    {
        readonly Mock<ILogger<ServiceExceptionLogger>> logger;
        readonly ServiceExceptionLogger subject;

        public ServiceExceptionLoggerTests()
        {
            this.logger = new Mock<ILogger<ServiceExceptionLogger>>();
            this.subject = new ServiceExceptionLogger(this.logger.Object);
        }

        [Fact]
        public async Task HandleExceptionAsync_WithValidArgs_ShouldInvokeLogger()
        {
            var service = typeof(FakeBackgroundService);
            var ex = new Exception();

            await this.subject.HandleExceptionAsync(service, ex);

            this.logger.Verify(
                l => l.Log(
                    LogLevel.Critical,
                    0,
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == $"Fatal error occurred in {service}."),
                    ex,
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once());
        }
    }
}
