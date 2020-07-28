namespace Zsharp.LightweightIndexer.Postgres.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Xunit;
    using IDbContextFactory = Zsharp.Entity.IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext>;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.subject = new ServiceCollection();
        }

        [Fact]
        public void AddLightweightIndexerPostgresDbContext_WithInvalidOptions_ShouldThrowWhenResolvingRegisteredServices()
        {
            // Act.
            this.subject.AddLightweightIndexerPostgresDbContext(options =>
            {
                options.ConnectionString = null;
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IDbContextFactory>());
        }

        [Fact]
        public void AddLightweightIndexerPostgresDbContext_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddLightweightIndexerPostgresDbContext(options =>
            {
                options.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            services.GetRequiredService<IDbContextFactory>();
        }
    }
}
