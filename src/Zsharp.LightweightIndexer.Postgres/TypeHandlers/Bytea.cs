namespace Zsharp.LightweightIndexer.Postgres.TypeHandlers
{
    using System.Threading.Tasks;
    using NBitcoin;
    using Npgsql;
    using Npgsql.BackendMessages;
    using Npgsql.PostgresTypes;
    using Npgsql.TypeHandling;

    sealed class Bytea : Npgsql.TypeHandlers.ByteaHandler, INpgsqlTypeHandler<uint256>
    {
        public Bytea(PostgresType postgresType)
            : base(postgresType)
        {
        }

        public int ValidateAndGetLength(uint256 value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            return 32;
        }

        public Task Write(
            uint256 value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async)
        {
            return this.Write(value.ToBytes(false), buf, lengthCache, parameter, async);
        }

        async ValueTask<uint256> INpgsqlTypeHandler<uint256>.Read(
            NpgsqlReadBuffer buf,
            int len,
            bool async,
            FieldDescription? fieldDescription)
        {
            var data = await this.Read(buf, len, async, fieldDescription);

            return new uint256(data, false);
        }
    }
}
