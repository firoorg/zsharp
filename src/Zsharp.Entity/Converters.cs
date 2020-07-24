namespace Zsharp.Entity
{
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NBitcoin;

    /// <remarks>
    /// All of converters should not used on any columns that need to create an index on it, including primary key; due
    /// to it will cause Entity Framework to do any operations that involved on the those indexes on the client side.
    /// That will cause those operations very slow, it is even slower than full table scan on the server side.
    ///
    /// The right way is creating a custom type mapper for the database provider instead. With this way Entity Framework
    /// can utilize indexes on the server side. You can see here for the starting point to create a custom type mapper:
    /// https://stackoverflow.com/a/58591235/1829232
    /// </remarks>
    public static class Converters
    {
        public static readonly ValueConverter<Script, byte[]> ScriptToBytes = new ValueConverter<Script, byte[]>(
            v => v.ToBytes(true),
            v => Script.FromBytesUnsafe(v));

        public static readonly ValueConverter<Target, int> TargetToInt32 = new ValueConverter<Target, int>(
            v => (int)v.ToCompact(),
            v => new Target((uint)v));

        public static readonly ValueConverter<uint256, byte[]> UInt256ToBytes = new ValueConverter<uint256, byte[]>(
            v => v.ToBytes(false),
            v => new uint256(v, false),
            new ConverterMappingHints(size: 32));
    }
}
