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
    using Zsharp.Entity;
    using DbContext = Zsharp.LightweightIndexer.Entity.DbContext;

    sealed class FakeDbContextFactory : IDbContextFactory<DbContext>, IDisposable
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

        public async ValueTask<DbContext> CreateAsync(CancellationToken cancellationToken = default)
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

            return new FakeDbContext(builder.Options);
        }

        public void Dispose()
        {
            this.connection.Dispose();
            this.logger.Dispose();
        }
    }
}
