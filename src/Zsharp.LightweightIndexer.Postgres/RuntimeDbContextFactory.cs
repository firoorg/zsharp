namespace Zsharp.LightweightIndexer.Postgres
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Zsharp.Entity;

    sealed class RuntimeDbContextFactory :
        DbContextFactory,
        IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext>
    {
        readonly DbContextOptions options;

        public RuntimeDbContextFactory(IOptions<DbContextOptions> options, IServiceProvider services)
        {
            this.options = options.Value;
            this.ServiceProvider = services;
        }

        protected override string ConnectionString => this.options.ConnectionString;

        protected override IServiceProvider ServiceProvider { get; }

        public async ValueTask<Entity.DbContext> CreateAsync(CancellationToken cancellationToken = default)
        {
            return await this.CreateDbContextAsync(cancellationToken);
        }
    }
}
