namespace Zsharp.Elysium
{
    using System;
    using System.Buffers;
    using System.Buffers.Binary;
    using System.Collections.Generic;
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

        public Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ref SequenceReader<byte> reader)
        {
            ushort version, type;

            using (var memory = MemoryPool<byte>.Shared.Rent(4))
            {
                var buffer = memory.Memory.Span.Slice(0, 4);

                if (!reader.TryCopyTo(buffer))
                {
                    throw new TransactionSerializationException("Incomplete data.");
                }

                reader.Advance(4);

                version = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(0));
                type = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(2));
            }

            if (!this.serializers.TryGetValue(type, out var serializer))
            {
                throw new TransactionSerializationException("Unknow transaction.");
            }

            try
            {
                return serializer.Deserialize(sender, receiver, ref reader, version);
            }
            catch (ArgumentException ex) when (ex.ParamName == "version")
            {
                throw new TransactionSerializationException("Unknow version.", ex);
            }
        }

        public void Serialize(IBufferWriter<byte> writer, Transaction transaction)
        {
            if (!this.serializers.TryGetValue(transaction.Id, out var serializer))
            {
                throw new ArgumentException("The transaction is not supported.", nameof(transaction));
            }

            BinaryPrimitives.WriteUInt16BigEndian(writer.GetSpan(2), Convert.ToUInt16(transaction.Version));
            writer.Advance(2);

            BinaryPrimitives.WriteUInt16BigEndian(writer.GetSpan(2), Convert.ToUInt16(transaction.Id));
            writer.Advance(2);

            serializer.Serialize(writer, transaction);
        }
    }
}
