namespace Zsharp.LightweightIndexer.Postgres
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Zsharp.Entity;

    public sealed class RuntimeDbContextFactory :
        DbContextFactory,
        IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext>
    {
        readonly DbContextOptions options;

        public RuntimeDbContextFactory(IOptions<DbContextOptions> options)
        {
            this.options = options.Value;
        }

        protected override string ConnectionString => this.options.ConnectionString;

        public ValueTask<Entity.DbContext> CreateAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<Entity.DbContext>(this.CreateDbContext());
        }
    }
}
