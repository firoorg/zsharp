namespace Zsharp.ServiceModel.AspNetCore.Tests
{
    using System;
    using Xunit;

    public sealed class ServiceExceptionFeatureTests
    {
        readonly Exception exception;
        readonly ServiceError error;
        readonly ServiceExceptionFeature subject;

        public ServiceExceptionFeatureTests()
        {
            this.exception = new Exception();
            this.error = new ServiceError(typeof(BackgroundService), this.exception);
            this.subject = new ServiceExceptionFeature(new[] { this.error });
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            var error = Assert.Single(this.subject.Exceptions);

            Assert.Equal(this.exception, error.Exception);
            Assert.Equal(typeof(BackgroundService), error.Service);
        }
    }
}
