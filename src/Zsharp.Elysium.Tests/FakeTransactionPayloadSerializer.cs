namespace Zsharp.Elysium.Tests
{
    using System;
    using System.IO;
    using Moq;
    using NBitcoin;

    sealed class FakeTransactionPayloadSerializer : TransactionPayloadSerializer
    {
        public FakeTransactionPayloadSerializer(int transactionId)
        {
            this.StubbedDeserialize = new Mock<Func<BitcoinAddress?, BitcoinAddress?, byte[], int, Elysium.Transaction>>();
            this.StubbedSerialize = new Mock<Action<MemoryStream, Elysium.Transaction>>();
            this.TransactionId = transactionId;
        }

        public Mock<Func<BitcoinAddress?, BitcoinAddress?, byte[], int, Elysium.Transaction>> StubbedDeserialize { get; }

        public Mock<Action<MemoryStream, Elysium.Transaction>> StubbedSerialize { get; }

        public override int TransactionId { get; }

        public override Elysium.Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ReadOnlySpan<byte> data,
            int version)
        {
            return this.StubbedDeserialize.Object(sender, receiver, data.ToArray(), version);
        }

        public override void Serialize(MemoryStream output, Elysium.Transaction transaction)
        {
            this.StubbedSerialize.Object(output, transaction);
        }
    }
}
