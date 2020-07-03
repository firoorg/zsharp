namespace Zsharp.Elysium.Tests
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.IO;

    static class RawTransaction
    {
        public static MemoryStream Create(int type, int version)
        {
            var data = new MemoryStream();

            try
            {
                using (var header = MemoryPool<byte>.Shared.Rent(4))
                {
                    var output = header.Memory.Span.Slice(0, 4);

                    BinaryPrimitives.WriteUInt16BigEndian(output.Slice(0), Convert.ToUInt16(version));
                    BinaryPrimitives.WriteUInt16BigEndian(output.Slice(2), Convert.ToUInt16(type));

                    data.Write(output);
                }
            }
            catch
            {
                data.Dispose();
                throw;
            }

            return data;
        }

        public static void WritePropertyId(Stream output, PropertyId id) => WritePropertyId(output, id.Value);

        public static void WritePropertyId(Stream output, long id)
        {
            using (var buffer = MemoryPool<byte>.Shared.Rent(4))
            {
                var data = buffer.Memory.Span.Slice(0, 4);
                BinaryPrimitives.WriteUInt32BigEndian(data, Convert.ToUInt32(id));
                output.Write(data);
            }
        }

        public static void WritePropertyAmount(Stream output, TokenAmount amount) =>
            WritePropertyAmount(output, amount.Value);

        public static void WritePropertyAmount(Stream output, long amount)
        {
            using (var buffer = MemoryPool<byte>.Shared.Rent(8))
            {
                var data = buffer.Memory.Span.Slice(0, 8);
                BinaryPrimitives.WriteInt64BigEndian(data, amount);
                output.Write(data);
            }
        }
    }
}
