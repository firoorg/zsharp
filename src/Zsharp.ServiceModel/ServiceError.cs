namespace Zsharp.ServiceModel
{
    using System;

    public sealed class ServiceError
    {
        public ServiceError(Type service, Exception exception)
        {
            this.Service = service;
            this.Exception = exception;
        }

        public Exception Exception { get; }

        public Type Service { get; }
    }
}
