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
            if (data.Length < 4)
            {
                throw new TransactionSerializationException("Not enough data.");
            }

            var version = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(0));
            var type = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(2));

            if (!this.serializers.TryGetValue(type, out var serializer))
            {
                throw new TransactionSerializationException("Unknow transaction.");
            }

            return serializer.Deserialize(sender, receiver, data.Slice(4), version);
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

                BinaryPrimitives.WriteUInt16BigEndian(output.Slice(0), (ushort)transaction.Version);
                BinaryPrimitives.WriteUInt16BigEndian(output.Slice(2), (ushort)transaction.Id);

                buffer.Write(output);
            }

            serializer.Serialize(buffer, transaction);

            return new ArraySegment<byte>(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
        }
    }
}
