namespace Zsharp.Elysium
{
    using System;
    using System.Buffers;
    using NBitcoin;

    public interface ITransactionSerializer
    {
        /// <summary>
        /// Deserialize a <see cref="Transaction"/> from a data reading from <paramref name="reader"/>.
        /// </summary>
        /// <param name="sender">
        /// The source address of the transaction.
        /// </param>
        /// <param name="receiver">
        /// The destination address of the transaction.
        /// </param>
        /// <param name="reader">
        /// The reader to provide a data to deserialize.
        /// </param>
        /// <returns>
        /// A deserialized transaction.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="receiver"/> is <c>null</c> on the transaction that required it.
        /// </exception>
        /// <exception cref="TransactionSerializationException">
        /// <paramref name="reader"/> contains invalid data.
        /// </exception>
        Transaction Deserialize(BitcoinAddress? sender, BitcoinAddress? receiver, ref SequenceReader<byte> reader);

        /// <summary>
        /// Serialize <paramref name="transaction"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer to write the serialized transaction.
        /// </param>
        /// <param name="transaction">
        /// The transaction to serialize.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="transaction"/> is not supported.
        /// </exception>
        void Serialize(IBufferWriter<byte> writer, Transaction transaction);
    }
}
