namespace Zsharp.ServiceModel.Tests
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class ServiceExceptionCollectorTests
    {
        readonly ServiceExceptionCollector subject;

        public ServiceExceptionCollectorTests()
        {
            this.subject = new ServiceExceptionCollector();
        }

        [Fact]
        public async Task HandleExceptionAsync_WithValidArgs_ShouldCollectPassedException()
        {
            var ex1 = new Exception();
            var ex2 = new Exception();

            await this.subject.HandleExceptionAsync(typeof(FakeBackgroundService), ex1);
            await this.subject.HandleExceptionAsync(typeof(BackgroundService), ex2);

            Assert.Collection(
                this.subject.Exceptions,
                e =>
                {
                    Assert.Same(ex1, e.Exception);
                    Assert.Equal(typeof(FakeBackgroundService), e.Service);
                },
                e =>
                {
                    Assert.Same(ex2, e.Exception);
                    Assert.Equal(typeof(BackgroundService), e.Service);
                });
        }
    }
}
