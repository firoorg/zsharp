namespace Zsharp.LightweightIndexer.Entity
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext();
    }
}
