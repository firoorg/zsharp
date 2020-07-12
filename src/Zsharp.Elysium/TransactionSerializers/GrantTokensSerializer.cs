namespace Zsharp.Elysium.TransactionSerializers
{
    using System;
    using System.Buffers;
    using NBitcoin;
    using Zsharp.Elysium.Transactions;

    public sealed class GrantTokensSerializer : TransactionPayloadSerializer
    {
        public override int TransactionId => GrantTokensV0.StaticId;

        public override Elysium.Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ref SequenceReader<byte> reader,
            int version)
        {
            if (ReferenceEquals(sender, null))
            {
                throw new ArgumentNullException(nameof(sender));
            }

            // Deserialize payload.
            var property = DeserializePropertyId(ref reader);
            var amount = DeserializeTokenAmount(ref reader);

            if (property == null)
            {
                throw new TransactionSerializationException("Invalid property identifier.");
            }

            // Construct domain object.
            try
            {
                return version switch
                {
                    0 => new GrantTokensV0(sender, receiver, property, amount),
                    _ => throw new ArgumentOutOfRangeException(nameof(version)),
                };
            }
            catch (ArgumentOutOfRangeException ex) when (ex.ParamName == "amount")
            {
                throw new TransactionSerializationException("Invalid amount.", ex);
            }
        }

        public override void Serialize(IBufferWriter<byte> writer, Elysium.Transaction transaction)
        {
            var tx = transaction as GrantTokensV0;

            if (tx == null)
            {
                throw new ArgumentException("The transaction is not supported.", nameof(transaction));
            }

            SerializePropertyId(writer, tx.Property);
            SerializeTokenAmount(writer, tx.Amount);
        }
    }
}
