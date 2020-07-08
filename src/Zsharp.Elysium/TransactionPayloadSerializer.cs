namespace Zsharp.Elysium
{
    using System;
    using System.Buffers.Binary;
    using System.IO;
    using NBitcoin;

    public abstract class TransactionPayloadSerializer : ITransactionPayloadSerializer
    {
        protected TransactionPayloadSerializer()
        {
        }

        public abstract int TransactionId { get; }

        public static PropertyId DeserializePropertyId(ReadOnlySpan<byte> data)
        {
            var value = ReadUInt32BigEndian(data);

            try
            {
                return new PropertyId(value);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new TransactionSerializationException("Invalid property identifier.", ex);
            }
        }

        public static TokenAmount DeserializeTokenAmount(ReadOnlySpan<byte> data)
        {
            var value = ReadInt64BigEndian(data);

            return new TokenAmount(value);
        }

        public static void SerializePropertyId(Span<byte> destination, PropertyId id)
        {
            if (!BinaryPrimitives.TryWriteUInt32BigEndian(destination, Convert.ToUInt32(id.Value)))
            {
                throw new ArgumentException("The space is not enough.", nameof(destination));
            }
        }

        public static void SerializeTokenAmount(Span<byte> destination, TokenAmount amount)
        {
            if (!BinaryPrimitives.TryWriteInt64BigEndian(destination, amount.Value))
            {
                throw new ArgumentException("The space is not enough.", nameof(destination));
            }
        }

        public abstract Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ReadOnlySpan<byte> data,
            int version);

        public abstract void Serialize(MemoryStream output, Transaction transaction);

        static long ReadInt64BigEndian(ReadOnlySpan<byte> data)
        {
            try
            {
                return BinaryPrimitives.ReadInt64BigEndian(data);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("The data is not enough.", nameof(data), ex);
            }
        }

        static uint ReadUInt32BigEndian(ReadOnlySpan<byte> data)
        {
            try
            {
                return BinaryPrimitives.ReadUInt32BigEndian(data);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("The data is not enough.", nameof(data), ex);
            }
        }
    }
}
