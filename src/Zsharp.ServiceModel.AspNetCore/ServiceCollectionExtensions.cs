namespace Microsoft.Extensions.DependencyInjection
{
    using Zsharp.ServiceModel;
    using Zsharp.ServiceModel.AspNetCore;

    public static class ServiceCollectionExtensions
    {
        public static void AddServiceExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<ServiceExceptionHandler>();
            services.AddSingleton<IServiceExceptionHandler>(p => p.GetRequiredService<ServiceExceptionHandler>());
        }
    }
}
