namespace Zsharp.LightweightIndexer.Postgres.Tests
{
    using System;
    using Microsoft.Extensions.Options;
    using Zsharp.Entity;
    using Zsharp.LightweightIndexer.Entity;

    public sealed class BlockRepositoryTests : Zsharp.LightweightIndexer.Entity.Tests.BlockRepositoryTests
    {
        public BlockRepositoryTests()
            : base(CreateContextFactory())
        {
        }

        static IDbContextFactory<DbContext> CreateContextFactory()
        {
            var options = new DbContextOptions()
            {
                ConnectionString = Environment.GetEnvironmentVariable("ZSHARP_POSTGRES_CONNECTIONSTRING"),
            };

            if (options.ConnectionString == null)
            {
                options.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
            }

            return new RuntimeDbContextFactory(new OptionsWrapper<DbContextOptions>(options));
        }
    }
}
