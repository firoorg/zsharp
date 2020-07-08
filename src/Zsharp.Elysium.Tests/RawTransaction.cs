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
    }
}
