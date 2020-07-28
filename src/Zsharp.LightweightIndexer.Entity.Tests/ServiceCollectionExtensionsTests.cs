namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium;
    using Zsharp.Entity;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly Mock<IDbContextFactory<DbContext>> db;
        readonly Mock<ITransactionSerializer> serializer;
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.db = new Mock<IDbContextFactory<DbContext>>();
            this.serializer = new Mock<ITransactionSerializer>();
            this.subject = new ServiceCollection();

            this.subject.AddSingleton(Networks.Default.Mainnet);
            this.subject.AddSingleton(this.db.Object);
            this.subject.AddSingleton(this.serializer.Object);
        }

        [Fact]
        public void AddLightweightIndexerEntityRepository_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddLightweightIndexerEntityRepository();

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            services.GetRequiredService<IBlockRepository>();
        }
    }
}
