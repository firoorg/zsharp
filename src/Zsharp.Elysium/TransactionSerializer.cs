namespace Zsharp.Elysium
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NBitcoin;

    public sealed class TransactionSerializer : ITransactionSerializer
    {
        public const int MinSize = 4;

        readonly Dictionary<int, ITransactionPayloadSerializer> serializers;

        public TransactionSerializer(IEnumerable<ITransactionPayloadSerializer> serializers)
        {
            this.serializers = serializers.ToDictionary(s => s.TransactionId);
        }

        public Transaction Deserialize(BitcoinAddress? sender, BitcoinAddress? receiver, ReadOnlySpan<byte> data)
        {
            ushort version, type;

            try
            {
                version = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(0));
                type = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(2));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentException("The data is not enough.", nameof(data), ex);
            }

            if (!this.serializers.TryGetValue(type, out var serializer))
            {
                throw new TransactionSerializationException("Unknow transaction.");
            }

            try
            {
                return serializer.Deserialize(sender, receiver, data.Slice(4), version);
            }
            catch (ArgumentException ex) when (ex.ParamName == "version")
            {
                throw new TransactionSerializationException("Unknow version.", ex);
            }
        }

        public ArraySegment<byte> Serialize(Transaction transaction)
        {
            if (!this.serializers.TryGetValue(transaction.Id, out var serializer))
            {
                throw new ArgumentException("The transaction is not supported.", nameof(transaction));
            }

            using var buffer = new MemoryStream();

            using (var data = MemoryPool<byte>.Shared.Rent(4))
            {
                var output = data.Memory.Span.Slice(0, 4);

                BinaryPrimitives.WriteUInt16BigEndian(output.Slice(0), Convert.ToUInt16(transaction.Version));
                BinaryPrimitives.WriteUInt16BigEndian(output.Slice(2), Convert.ToUInt16(transaction.Id));

                buffer.Write(output);
            }

            serializer.Serialize(buffer, transaction);

            return new ArraySegment<byte>(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
        }
    }
}
