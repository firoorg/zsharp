namespace Zsharp.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public sealed class ServiceExceptionLogger : IServiceExceptionHandler
    {
        readonly ILogger logger;

        public ServiceExceptionLogger(ILogger<ServiceExceptionLogger> logger)
        {
            this.logger = logger;
        }

        public Task HandleExceptionAsync(
            Type service,
            Exception exception,
            CancellationToken cancellationToken = default)
        {
            this.logger.LogCritical(exception, "Fatal error occurred in {Service}.", service);

            return Task.CompletedTask;
        }
    }
}
