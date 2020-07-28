namespace Microsoft.Extensions.DependencyInjection
{
    using NBitcoin;
    using Zsharp.Bitcoin;

    public static class ServiceCollectionExtensions
    {
        public static void AddZcoin(this IServiceCollection services, NetworkType type)
        {
            services.AddSingleton<Network>(Networks.Default.GetNetwork(type));
        }
    }
}
