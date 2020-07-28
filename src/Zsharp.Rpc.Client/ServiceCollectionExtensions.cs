namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Microsoft.Extensions.Options;
    using NBitcoin;
    using Zsharp.Elysium;
    using Zsharp.Rpc.Client;

    public static class ServiceCollectionExtensions
    {
        public static void AddZcoinRpcClient(this IServiceCollection services, Action<RpcClientOptions> options)
        {
            services.AddOptions<RpcClientOptions>().Configure(options).ValidateDataAnnotations();
            services.AddSingleton<IRpcClientFactory>(CreateFactory);
        }

        static IRpcClientFactory CreateFactory(IServiceProvider services)
        {
            var options = services.GetRequiredService<IOptions<RpcClientOptions>>().Value;
            var network = services.GetRequiredService<Network>();
            var serializer = services.GetRequiredService<ITransactionSerializer>();

            return new RpcClientFactory(network, options.ServerUrl, options.Credential, serializer);
        }
    }
}
