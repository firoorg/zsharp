namespace Zsharp.LightweightIndexer.Postgres
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Zsharp.LightweightIndexer.Entity;

    sealed class RuntimeDbContextFactory : DbContextFactory, IDbContextFactory
    {
        readonly DbContextOptions options;

        public RuntimeDbContextFactory(IOptions<DbContextOptions> options, IServiceProvider services)
        {
            this.options = options.Value;
            this.ServiceProvider = services;
        }

        protected override string ConnectionString => this.options.ConnectionString;

        protected override IServiceProvider ServiceProvider { get; }

        public async ValueTask<Entity.DbContext> CreateAsync(
            IsolationLevel? isolation = null,
            CancellationToken cancellationToken = default)
        {
            var context = await this.CreateDbContextAsync(cancellationToken);

            try
            {
                if (isolation != null)
                {
                    await context.Database.BeginTransactionAsync(isolation.Value, cancellationToken);
                }
            }
            catch
            {
                await context.DisposeAsync();
                throw;
            }

            return context;
        }
    }
}
