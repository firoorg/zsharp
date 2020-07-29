namespace Zsharp.LightweightIndexer.Postgres
{
    using System;
    using Microsoft.EntityFrameworkCore.Design;

    public sealed class DesignTimeDbContextFactory : DbContextFactory, IDesignTimeDbContextFactory<DbContext>
    {
        public DesignTimeDbContextFactory()
        {
            var @override = Environment.GetEnvironmentVariable("ZSHARP_POSTGRES_CONNECTIONSTRING");
            var @default = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";

            this.ConnectionString = @override ?? @default;
        }

        protected override string ConnectionString { get; }

        public DbContext CreateDbContext(string[] args)
        {
            return this.CreateDbContext();
        }
    }
}
