namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Storage;
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

            services.AddEntityFrameworkPlugins();
        }

        internal static void AddEntityFrameworkPlugins(this IServiceCollection services)
        {
            var entityServicesBuilder = new EntityFrameworkRelationalServicesBuilder(services);

            entityServicesBuilder.TryAddProviderSpecificServices(m =>
            {
                m.TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, TypeMappingSourcePlugin>();
            });
        }
    }
}
