namespace Zsharp.LightweightIndexer.Postgres
{
    using Microsoft.EntityFrameworkCore.Design;

    public sealed class DesignTimeDbContextFactory : DbContextFactory, IDesignTimeDbContextFactory<DbContext>
    {
        public DesignTimeDbContextFactory()
        {
            this.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
        }

        protected override string ConnectionString { get; }

        public DbContext CreateDbContext(string[] args)
        {
            return this.CreateDbContext();
        }
    }
}
