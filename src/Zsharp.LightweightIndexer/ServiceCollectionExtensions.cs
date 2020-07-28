namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Microsoft.Extensions.Options;
    using Zsharp.LightweightIndexer;
    using Zsharp.Rpc.Client;

    public static class ServiceCollectionExtensions
    {
        public static void AddLightweightIndexer(
            this IServiceCollection services,
            Action<LightweightIndexerOptions> options)
        {
            services.AddOptions<LightweightIndexerOptions>().Configure(options).ValidateDataAnnotations();
            services.AddTransient<IBlockRetriever>(CreateBlockRetriever);
            services.AddSingleton<BlockSynchronizer>();
            services.AddHostedService(p => p.GetRequiredService<BlockSynchronizer>());
            services.AddSingleton<IBlockSynchronizer>(p => p.GetRequiredService<BlockSynchronizer>());
        }

        static IBlockRetriever CreateBlockRetriever(IServiceProvider services)
        {
            var options = services.GetRequiredService<IOptions<LightweightIndexerOptions>>().Value;
            var rpc = services.GetRequiredService<IRpcClientFactory>();

            return new BlockRetriever(rpc, options.BlockPublisherAddress);
        }
    }
}
