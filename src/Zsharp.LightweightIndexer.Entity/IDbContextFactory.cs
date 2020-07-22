namespace Zsharp.LightweightIndexer.Entity
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDbContextFactory
    {
        ValueTask<DbContext> CreateAsync(
            IsolationLevel? isolation = IsolationLevel.Unspecified,
            CancellationToken cancellationToken = default);
    }
}
