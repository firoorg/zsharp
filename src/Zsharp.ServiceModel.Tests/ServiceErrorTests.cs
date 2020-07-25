namespace Zsharp.ServiceModel.Tests
{
    using System;
    using Xunit;

    public sealed class ServiceErrorTests
    {
        readonly Type service;
        readonly Exception exception;
        readonly ServiceError subject;

        public ServiceErrorTests()
        {
            this.service = typeof(FakeBackgroundService);
            this.exception = new Exception();
            this.subject = new ServiceError(this.service, this.exception);
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Same(this.exception, this.subject.Exception);
            Assert.Same(this.service, this.subject.Service);
        }
    }
}
