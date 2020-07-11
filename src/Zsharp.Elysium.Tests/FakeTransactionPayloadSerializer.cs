namespace Zsharp.Elysium.Tests
{
    using System;
    using System.Buffers;
    using Moq;
    using NBitcoin;

    sealed class FakeTransactionPayloadSerializer : TransactionPayloadSerializer
    {
        public FakeTransactionPayloadSerializer(int transactionId)
        {
            this.StubbedDeserialize = new Mock<Func<BitcoinAddress?, BitcoinAddress?, byte[], int, Elysium.Transaction>>();
            this.StubbedSerialize = new Mock<Action<IBufferWriter<byte>, Elysium.Transaction>>();
            this.TransactionId = transactionId;
        }

        public Mock<Func<BitcoinAddress?, BitcoinAddress?, byte[], int, Elysium.Transaction>> StubbedDeserialize { get; }

        public Mock<Action<IBufferWriter<byte>, Elysium.Transaction>> StubbedSerialize { get; }

        public override int TransactionId { get; }

        public override Elysium.Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ref SequenceReader<byte> reader,
            int version)
        {
            var length = Convert.ToInt32(reader.Remaining);

            using (var memory = MemoryPool<byte>.Shared.Rent(length))
            {
                var buffer = memory.Memory.Span.Slice(0, length);

                reader.TryCopyTo(buffer);
                reader.Advance(length);

                return this.StubbedDeserialize.Object(sender, receiver, buffer.ToArray(), version);
            }
        }

        public override void Serialize(IBufferWriter<byte> writer, Elysium.Transaction transaction)
        {
            this.StubbedSerialize.Object(writer, transaction);
        }
    }
}
