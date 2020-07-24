namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Zsharp.LightweightIndexer.Postgres;
    using IDbContextFactory = Zsharp.Entity.IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext>;

    public static class LightweightIndexerPostgresDbContextServiceCollectionExtensions
    {
        public static void AddLightweightIndexerPostgresDbContext(
            this IServiceCollection services,
            Action<DbContextOptions> options)
        {
            services.AddSingleton<IDbContextFactory, RuntimeDbContextFactory>();
            services.AddOptions<DbContextOptions>().Configure(options).ValidateDataAnnotations();
        }
    }
}
