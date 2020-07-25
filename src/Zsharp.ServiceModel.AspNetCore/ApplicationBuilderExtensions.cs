namespace Microsoft.AspNetCore.Builder
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Zsharp.ServiceModel.AspNetCore;

    public static class ApplicationBuilderExtensions
    {
        public static void UseServiceExceptionHandler(this IApplicationBuilder app, string rendererPath)
        {
            app.Use((context, next) =>
            {
                var handler = context.RequestServices.GetRequiredService<ServiceExceptionHandler>();
                var exceptions = handler.Collector.Exceptions;

                if (exceptions.Any())
                {
                    context.Features.Set(new ServiceExceptionFeature(exceptions));
                    context.Request.Path = rendererPath;
                }

                return next();
            });
        }
    }
}
