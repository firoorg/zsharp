namespace Zsharp.Entity.Postgres.TypeMapping
{
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NBitcoin;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
    using NpgsqlTypes;

    public sealed class UInt256 : NpgsqlTypeMapping
    {
        public const NpgsqlDbType DatabaseType = NpgsqlDbType.Bytea;

        static readonly MethodInfo? Parse = typeof(uint256).GetRuntimeMethod(
            nameof(uint256.Parse),
            new[] { typeof(string) });

        public UInt256()
            : base("bytea", typeof(uint256), DatabaseType)
        {
        }

        private UInt256(RelationalTypeMappingParameters parameters)
            : base(parameters, DatabaseType)
        {
        }

        public override RelationalTypeMapping Clone(string storeType, int? size)
        {
            return new UInt256(this.Parameters.WithStoreTypeAndSize(storeType, size));
        }

        public override CoreTypeMapping Clone(ValueConverter converter)
        {
            return new UInt256(this.Parameters.WithComposedConverter(converter));
        }

        public override Expression GenerateCodeLiteral(object value)
        {
            return Expression.Call(Parse, Expression.Constant(value.ToString()));
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            return new UInt256(parameters);
        }

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            return $@"'\x{value}'";
        }
    }
}
