namespace Zsharp.Entity
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public interface IDbContextFactory<T>
        where T : DbContext
    {
        ValueTask<T> CreateAsync(CancellationToken cancellationToken = default);
    }
}
