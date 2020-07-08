namespace Zsharp.Elysium
{
    using System;
    using NBitcoin;

    public interface ITransactionSerializer
    {
        /// <summary>
        /// Deserialize a <see cref="Transaction"/> from <paramref name="data"/>.
        /// </summary>
        /// <param name="sender">
        /// The source address of the transaction.
        /// </param>
        /// <param name="receiver">
        /// The destination address of the transaction.
        /// </param>
        /// <param name="data">
        /// The serialized data to deserialize.
        /// </param>
        /// <returns>
        /// A deserialized transaction.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="receiver"/> is <c>null</c> on the transaction that required it.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="data"/> is not enought.
        /// </exception>
        /// <exception cref="TransactionSerializationException">
        /// <paramref name="data"/> contains invalid data.
        /// </exception>
        Transaction Deserialize(BitcoinAddress? sender, BitcoinAddress? receiver, ReadOnlySpan<byte> data);

        /// <summary>
        /// Serialize <paramref name="transaction"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction to serialize.
        /// </param>
        /// <returns>
        /// The serialized data.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="transaction"/> is not supported.
        /// </exception>
        ArraySegment<byte> Serialize(Transaction transaction);
    }
}
