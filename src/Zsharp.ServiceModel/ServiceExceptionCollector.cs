namespace Zsharp.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ServiceExceptionCollector : IServiceExceptionHandler
    {
        readonly List<ServiceError> exceptions;

        public ServiceExceptionCollector()
        {
            this.exceptions = new List<ServiceError>();
        }

        public IEnumerable<ServiceError> Exceptions
        {
            get
            {
                lock (this.exceptions)
                {
                    return this.exceptions.ToArray();
                }
            }
        }

        public Task HandleExceptionAsync(
            Type service,
            Exception exception,
            CancellationToken cancellationToken = default)
        {
            var error = new ServiceError(service, exception);

            lock (this.exceptions)
            {
                this.exceptions.Add(error);
            }

            return Task.CompletedTask;
        }
    }
}
