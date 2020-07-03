namespace Zsharp.Elysium
{
    using System;
    using NBitcoin;

    public interface ITransactionSerializer
    {
        Transaction Deserialize(BitcoinAddress? sender, BitcoinAddress? receiver, ReadOnlySpan<byte> data);

        ArraySegment<byte> Serialize(Transaction transaction);
    }
}
