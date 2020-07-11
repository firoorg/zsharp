namespace Zsharp.Elysium.Tests
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;

    static class SerializationTesting
    {
        public static ArrayBufferWriter<byte> CreateWriter(int id, int version)
        {
            var writer = new ArrayBufferWriter<byte>();

            BinaryPrimitives.WriteUInt16BigEndian(writer.GetSpan(2), Convert.ToUInt16(version));
            writer.Advance(2);

            BinaryPrimitives.WriteUInt16BigEndian(writer.GetSpan(2), Convert.ToUInt16(id));
            writer.Advance(2);

            return writer;
        }

        public static SequenceReader<byte> CreateReader(byte[] data)
        {
            return new SequenceReader<byte>(new ReadOnlySequence<byte>(data));
        }

        public static SequenceReader<byte> CreateReader(ArrayBufferWriter<byte> writer)
        {
            return new SequenceReader<byte>(new ReadOnlySequence<byte>(writer.WrittenMemory));
        }
    }
}
