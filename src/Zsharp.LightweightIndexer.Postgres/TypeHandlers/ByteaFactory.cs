namespace Zsharp.LightweightIndexer.Postgres.TypeHandlers
{
    using Npgsql;
    using Npgsql.PostgresTypes;
    using Npgsql.TypeHandling;

    sealed class ByteaFactory : NpgsqlTypeHandlerFactory<byte[]>
    {
        public override NpgsqlTypeHandler<byte[]> Create(PostgresType pgType, NpgsqlConnection conn)
        {
            return new Bytea(pgType);
        }
    }
}
