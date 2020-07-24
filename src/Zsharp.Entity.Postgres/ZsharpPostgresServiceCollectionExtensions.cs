namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Storage;
    using Zsharp.Entity.Postgres;

    public static class ZsharpPostgresServiceCollectionExtensions
    {
        public static void AddEntityFrameworkZcoinPostgresPlugins(this IServiceCollection services)
        {
            var builder = new EntityFrameworkRelationalServicesBuilder(services);

            builder.TryAddProviderSpecificServices(m =>
            {
                m.TryAddSingletonEnumerable<IRelationalTypeMappingSourcePlugin, TypeMappingSourcePlugin>();
            });
        }
    }
}
