namespace Zsharp.Entity.Postgres
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using NBitcoin;
    using Npgsql;
    using Npgsql.TypeMapping;
    using NpgsqlTypes;
    using Zsharp.Entity.Postgres.TypeHandlers;

    public sealed class DbConnectionInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.DbConnectionInterceptor
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

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            this.RegisterCustomTypeMappers((NpgsqlConnection)connection);
        }

        public override Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            this.RegisterCustomTypeMappers((NpgsqlConnection)connection);

            return Task.CompletedTask;
        }

        void RegisterCustomTypeMappers(NpgsqlConnection connection)
        {
            connection.TypeMapper.AddMapping(UInt256Mapping);
        }
    }
}
