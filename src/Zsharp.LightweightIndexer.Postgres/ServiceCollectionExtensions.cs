namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Zsharp.LightweightIndexer.Postgres;
    using IDbContextFactory = Zsharp.Entity.IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext>;

    public static class ServiceCollectionExtensions
    {
        public static void AddLightweightIndexerPostgresDbContext(
            this IServiceCollection services,
            Action<DbContextOptions> options)
        {
            services.AddOptions<DbContextOptions>().Configure(options).ValidateDataAnnotations();
            services.AddSingleton<IDbContextFactory, RuntimeDbContextFactory>();
        }
    }
}
