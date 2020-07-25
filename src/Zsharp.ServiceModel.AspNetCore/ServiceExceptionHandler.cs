namespace Zsharp.ServiceModel.AspNetCore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public sealed class ServiceExceptionHandler : IServiceExceptionHandler
    {
        public ServiceExceptionHandler(ILoggerFactory loggerFactory)
        {
            this.Collector = new ServiceExceptionCollector();
            this.Logger = new ServiceExceptionLogger(loggerFactory.CreateLogger<ServiceExceptionLogger>());
        }

        public ServiceExceptionCollector Collector { get; }

        public ServiceExceptionLogger Logger { get; }

        public async Task HandleExceptionAsync(
            Type service,
            Exception exception,
            CancellationToken cancellationToken = default)
        {
            // We cannot do IApplicationLifetime.StopApplication() here due to there is a race condition if background
            // task error too early. So we use another approach.
            await this.Logger.HandleExceptionAsync(service, exception, cancellationToken);
            await this.Collector.HandleExceptionAsync(service, exception, cancellationToken);
        }
    }
}
