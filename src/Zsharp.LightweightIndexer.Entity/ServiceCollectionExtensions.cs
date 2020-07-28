namespace Microsoft.Extensions.DependencyInjection
{
    using Zsharp.LightweightIndexer;
    using Zsharp.LightweightIndexer.Entity;

    public static class ServiceCollectionExtensions
    {
        public static void AddLightweightIndexerEntityRepository(this IServiceCollection services)
        {
            services.AddSingleton<IBlockRepository, BlockRepository>();
        }
    }
}
