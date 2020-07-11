namespace Zsharp.Elysium
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.Text;
    using NBitcoin;
    using Zsharp.Elysium.Transactions;

    public abstract class TransactionPayloadSerializer : ITransactionPayloadSerializer
    {
        protected TransactionPayloadSerializer()
        {
        }

        public abstract int TransactionId { get; }

        public static PrivateTransactionStatus DeserializePrivateTransactionStatus(ref SequenceReader<byte> reader)
        {
            var value = ReadByte(ref reader);

            if (!Enum.IsDefined(typeof(PrivateTransactionStatus), Convert.ToInt32(value)))
            {
                throw new TransactionSerializationException("Invalid private transaction status.");
            }

            return (PrivateTransactionStatus)value;
        }

        public static PropertyId? DeserializePropertyId(ref SequenceReader<byte> reader)
        {
            var value = ReadUInt32BigEndian(ref reader);

            return (value == 0) ? null : new PropertyId(value);
        }

        public static PropertyType DeserializePropertyType(ref SequenceReader<byte> reader)
        {
            var value = ReadByte(ref reader);

            if (!Enum.IsDefined(typeof(PropertyType), Convert.ToInt32(value)))
            {
                throw new TransactionSerializationException("Invalid property type.");
            }

            return (PropertyType)value;
        }

        public static string DeserializeString(ref SequenceReader<byte> reader)
        {
            if (reader.Remaining == 0)
            {
                throw new TransactionSerializationException("Incomplete data.");
            }

            ReadOnlySpan<byte> data;

            if (!reader.TryReadTo(out data, 0))
            {
                throw new TransactionSerializationException("Invalid string.");
            }

            return Encoding.UTF8.GetString(data);
        }

        public static TokenAmount DeserializeTokenAmount(ref SequenceReader<byte> reader)
        {
            var value = ReadInt64BigEndian(ref reader);

            return new TokenAmount(value);
        }

        public static TokenType DeserializeTokenType(ref SequenceReader<byte> reader)
        {
            var value = ReadUInt16BigEndian(ref reader);

            if (!Enum.IsDefined(typeof(TokenType), Convert.ToInt32(value)))
            {
                throw new TransactionSerializationException("Invalid token type.");
            }

            return (TokenType)value;
        }

        public static void SerializePrivateTransactionStatus(IBufferWriter<byte> writer, PrivateTransactionStatus value)
        {
            writer.GetSpan(1)[0] = Convert.ToByte(value);
            writer.Advance(1);
        }

        public static void SerializePropertyId(IBufferWriter<byte> writer, PropertyId? value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(
                writer.GetSpan(4),
                (value != null) ? Convert.ToUInt32(value.Value) : 0);
            writer.Advance(4);
        }

        public static void SerializePropertyType(IBufferWriter<byte> writer, PropertyType value)
        {
            writer.GetSpan(1)[0] = Convert.ToByte(value);
            writer.Advance(1);
        }

        public static void SerializeString(IBufferWriter<byte> writer, string value)
        {
            var encoded = Encoding.UTF8.GetBytes(value);

            writer.Write(encoded);
            writer.GetSpan(1)[0] = 0;
            writer.Advance(1);
        }

        public static void SerializeTokenAmount(IBufferWriter<byte> writer, TokenAmount value)
        {
            BinaryPrimitives.WriteInt64BigEndian(writer.GetSpan(8), value.Value);
            writer.Advance(8);
        }

        public static void SerializeTokenType(IBufferWriter<byte> writer, TokenType value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(writer.GetSpan(2), Convert.ToUInt16(value));
            writer.Advance(2);
        }

        public abstract Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ref SequenceReader<byte> reader,
            int version);

        public abstract void Serialize(IBufferWriter<byte> writer, Transaction transaction);

        static byte ReadByte(ref SequenceReader<byte> reader)
        {
            if (!reader.TryRead(out var value))
            {
                throw new TransactionSerializationException("Incomplete data.");
            }

            return value;
        }

        static long ReadInt64BigEndian(ref SequenceReader<byte> reader)
        {
            using (var memory = MemoryPool<byte>.Shared.Rent(8))
            {
                var buffer = memory.Memory.Span.Slice(0, 8);

                if (!reader.TryCopyTo(buffer))
                {
                    throw new TransactionSerializationException("Incomplete data.");
                }

                reader.Advance(8);

                return BinaryPrimitives.ReadInt64BigEndian(buffer);
            }
        }

        static ushort ReadUInt16BigEndian(ref SequenceReader<byte> reader)
        {
            using (var memory = MemoryPool<byte>.Shared.Rent(2))
            {
                var buffer = memory.Memory.Span.Slice(0, 2);

                if (!reader.TryCopyTo(buffer))
                {
                    throw new TransactionSerializationException("Incomplete data.");
                }

                reader.Advance(buffer.Length);

                return BinaryPrimitives.ReadUInt16BigEndian(buffer);
            }
        }

        static uint ReadUInt32BigEndian(ref SequenceReader<byte> reader)
        {
            using (var memory = MemoryPool<byte>.Shared.Rent(4))
            {
                var buffer = memory.Memory.Span.Slice(0, 4);

                if (!reader.TryCopyTo(buffer))
                {
                    throw new TransactionSerializationException("Incomplete data.");
                }

                reader.Advance(buffer.Length);

                return BinaryPrimitives.ReadUInt32BigEndian(buffer);
            }
        }
    }
}
