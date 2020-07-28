namespace Zsharp.Elysium.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.subject = new ServiceCollection();
        }

        [Fact]
        public void AddElysiumSerializer_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddElysiumSerializer();

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            services.GetRequiredService<ITransactionSerializer>();
        }
    }
}
