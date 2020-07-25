namespace Zsharp.ServiceModel.AspNetCore
{
    using System.Collections.Generic;

    public sealed class ServiceExceptionFeature
    {
        public ServiceExceptionFeature(IEnumerable<ServiceError> exceptions)
        {
            this.Exceptions = exceptions;
        }

        public IEnumerable<ServiceError> Exceptions { get; }
    }
}
