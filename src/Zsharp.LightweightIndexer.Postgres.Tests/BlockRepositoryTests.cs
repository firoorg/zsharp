namespace Zsharp.LightweightIndexer.Postgres.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Zsharp.Entity;
    using Zsharp.LightweightIndexer.Entity;

    public sealed class BlockRepositoryTests : Zsharp.LightweightIndexer.Entity.Tests.BlockRepositoryTests
    {
        readonly ServiceProvider services;

        public BlockRepositoryTests()
            : this(SetupServices())
        {
        }

        private BlockRepositoryTests(ServiceProvider services)
            : base(services.GetRequiredService<IDbContextFactory<DbContext>>())
        {
            this.services = services;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.services.Dispose();
        }

        static ServiceProvider SetupServices()
        {
            var services = new ServiceCollection();

            services.AddEntityFrameworkNpgsql();
            services.AddLightweightIndexerPostgresDbContext(options =>
            {
                options.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
            });

            return services.BuildServiceProvider();
        }
    }
}
