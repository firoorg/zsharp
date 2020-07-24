namespace Zsharp.LightweightIndexer.Postgres
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NBitcoin;
    using Npgsql;
    using Npgsql.TypeMapping;
    using NpgsqlTypes;
    using Zsharp.LightweightIndexer.Postgres.TypeHandlers;

    public abstract class DbContextFactory
    {
        static readonly NpgsqlTypeMapping UInt256Mapping = new NpgsqlTypeMappingBuilder()
        {
            PgTypeName = "bytea",
            NpgsqlDbType = NpgsqlDbType.Bytea,
            ClrTypes = new[]
            {
                typeof(uint256),
                typeof(byte[]),
                typeof(ArraySegment<byte>),
                typeof(ReadOnlyMemory<byte>),
                typeof(Memory<byte>),
            },
            TypeHandlerFactory = new ByteaFactory(),
        }.Build();

        protected DbContextFactory()
        {
        }

        protected abstract string ConnectionString { get; }

        protected abstract IServiceProvider ServiceProvider { get; }

        protected async Task<DbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            var builder = new DbContextOptionsBuilder<Zsharp.LightweightIndexer.Entity.DbContext>();
            var connection = new NpgsqlConnection(this.ConnectionString);

            try
            {
                await connection.OpenAsync(cancellationToken);

                connection.TypeMapper.AddMapping(UInt256Mapping);

                builder.UseInternalServiceProvider(this.ServiceProvider);
                builder.UseNpgsql(connection);

                return new DbContext(builder.Options);
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }
    }
}
