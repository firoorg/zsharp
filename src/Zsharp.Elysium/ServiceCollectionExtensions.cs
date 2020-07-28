namespace Microsoft.Extensions.DependencyInjection
{
    using Zsharp.Elysium;
    using Zsharp.Elysium.TransactionSerializers;

    public static class ServiceCollectionExtensions
    {
        public static void AddElysiumSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ITransactionPayloadSerializer, CreateManagedPropertySerializer>();
            services.AddSingleton<ITransactionPayloadSerializer, GrantTokensSerializer>();
            services.AddSingleton<ITransactionPayloadSerializer, SimpleSendSerializer>();
            services.AddSingleton<ITransactionSerializer, TransactionSerializer>();
        }
    }
}
