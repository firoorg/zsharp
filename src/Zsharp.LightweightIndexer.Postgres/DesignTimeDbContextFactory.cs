namespace Zsharp.LightweightIndexer.Postgres
{
    using System;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.DependencyInjection;

    public sealed class DesignTimeDbContextFactory :
        DbContextFactory,
        IDesignTimeDbContextFactory<DbContext>,
        IDisposable
    {
        readonly ServiceProvider services;
        bool disposed;

        public DesignTimeDbContextFactory()
        {
            var services = new ServiceCollection();

            services.AddEntityFrameworkNpgsql();
            services.AddEntityFrameworkPlugins();

            this.ConnectionString = "Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres";
            this.services = services.BuildServiceProvider();
        }

        protected override string ConnectionString { get; }

        protected override IServiceProvider ServiceProvider => this.services;

        public DbContext CreateDbContext(string[] args)
        {
            return this.CreateDbContextAsync().Result;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.services.Dispose();

            this.disposed = true;
        }
    }
}
