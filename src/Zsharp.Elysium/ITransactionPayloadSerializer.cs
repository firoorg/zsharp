namespace Zsharp.Elysium
{
    using System.Buffers;
    using NBitcoin;

    public interface ITransactionPayloadSerializer
    {
        int TransactionId { get; }

        Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ref SequenceReader<byte> reader,
            int version);

        void Serialize(IBufferWriter<byte> writer, Transaction transaction);
    }
}
