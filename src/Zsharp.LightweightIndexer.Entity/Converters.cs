namespace Zsharp.LightweightIndexer.Entity
{
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NBitcoin;

    public static class Converters
    {
        public static readonly ValueConverter<Script, byte[]> ScriptToBytesConverter = new ValueConverter<Script, byte[]>(
            v => v.ToBytes(true),
            v => Script.FromBytesUnsafe(v));

        public static readonly ValueConverter<Target, int> TargetToInt32 = new ValueConverter<Target, int>(
            v => (int)v.ToCompact(),
            v => new Target((uint)v));

        public static readonly ValueConverter<uint256, byte[]> UInt256ToBytesConverter = new ValueConverter<uint256, byte[]>(
            v => v.ToBytes(false),
            v => new uint256(v, false),
            new ConverterMappingHints(size: 32));
    }
}
