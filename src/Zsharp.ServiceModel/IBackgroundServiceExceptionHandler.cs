namespace Zsharp.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBackgroundServiceExceptionHandler
    {
        ValueTask RunAsync(Type service, Exception exception, CancellationToken cancellationToken = default);
    }
}
