namespace Zsharp.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IServiceExceptionHandler
    {
        Task HandleExceptionAsync(Type service, Exception exception, CancellationToken cancellationToken = default);
    }
}
