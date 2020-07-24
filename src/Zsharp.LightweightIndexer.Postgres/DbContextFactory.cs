namespace Zsharp.LightweightIndexer.Postgres
{
    using Microsoft.EntityFrameworkCore;
    using DbContextOptionsBuilder = Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Zsharp.LightweightIndexer.Entity.DbContext>;

    public abstract class DbContextFactory
    {
        protected DbContextFactory()
        {
        }

        protected abstract string ConnectionString { get; }

        protected DbContext CreateDbContext(DbContextOptionsBuilder? builder = null)
        {
            if (builder == null)
            {
                builder = new DbContextOptionsBuilder();
            }

            builder.UseNpgsql(this.ConnectionString, options =>
            {
                options.UseZcoin();
            });

            return new DbContext(builder.Options);
        }
    }
}
