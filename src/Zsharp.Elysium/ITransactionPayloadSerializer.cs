namespace Zsharp.Elysium
{
    using System;
    using System.IO;
    using NBitcoin;

    public interface ITransactionPayloadSerializer
    {
        int TransactionId { get; }

        Transaction Deserialize(BitcoinAddress? sender, BitcoinAddress? receiver, ReadOnlySpan<byte> data, int version);

        void Serialize(MemoryStream output, Transaction transaction);
    }
}
