namespace Zsharp.LightweightIndexer.Entity.Tests
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Debug;
    using DbContext = Zsharp.LightweightIndexer.Entity.DbContext;

    sealed class FakeDbContextFactory : IDbContextFactory, IDisposable
    {
        readonly ILoggerFactory logger;
        readonly SqliteConnection connection;

        public FakeDbContextFactory()
        {
            try
            {
                var logger = new DebugLoggerProvider();

                try
                {
                    this.logger = new LoggerFactory(new[] { logger });
                }
                catch
                {
                    logger.Dispose();
                    throw;
                }

                this.connection = new SqliteConnection("DataSource=:memory:");
            }
            catch
            {
                this.logger?.Dispose();
                throw;
            }
        }

        public async ValueTask<DbContext> CreateAsync(
            IsolationLevel? isolation = null,
            CancellationToken cancellationToken = default)
        {
            var builder = new DbContextOptionsBuilder<DbContext>();

            if (this.connection.State == ConnectionState.Closed)
            {
                await this.connection.OpenAsync(cancellationToken);
            }

            builder.EnableDetailedErrors();
            builder.EnableSensitiveDataLogging();
            builder.UseSqlite(this.connection);
            builder.UseLoggerFactory(this.logger);

            var context = new FakeDbContext(builder.Options);

            try
            {
                if (isolation != null)
                {
                    await context.Database.BeginTransactionAsync(isolation.Value, cancellationToken);
                }
            }
            catch
            {
                context.Dispose();
                throw;
            }

            return context;
        }

        public void Dispose()
        {
            this.connection.Dispose();
            this.logger.Dispose();
        }
    }
}
