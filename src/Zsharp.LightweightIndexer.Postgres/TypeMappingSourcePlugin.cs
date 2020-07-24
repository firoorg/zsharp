namespace Zsharp.LightweightIndexer.Postgres
{
    using Microsoft.EntityFrameworkCore.Storage;
    using NBitcoin;

    sealed class TypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
    {
        static readonly RelationalTypeMapping UInt256Mapping = new TypeMapping.UInt256();

        public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            if (mappingInfo.ClrType == typeof(uint256))
            {
                return UInt256Mapping;
            }

            return null;
        }
    }
}
