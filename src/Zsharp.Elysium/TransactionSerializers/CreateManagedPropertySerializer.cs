namespace Zsharp.Elysium.TransactionSerializers
{
    using System;
    using System.Buffers;
    using NBitcoin;
    using Zsharp.Elysium.Transactions;

    public sealed class CreateManagedPropertySerializer : TransactionPayloadSerializer
    {
        public override int TransactionId => CreateManagedPropertyV0.StaticId;

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
            var type = DeserializePropertyType(ref reader);
            var tokenType = DeserializeTokenType(ref reader);
            var previousId = DeserializePropertyId(ref reader);
            var category = DeserializeString(ref reader);
            var subcategory = DeserializeString(ref reader);
            var name = DeserializeString(ref reader);
            var website = DeserializeString(ref reader);
            var description = DeserializeString(ref reader);
            PrivateTransactionStatus? privateTransactionStatus = null;

            if (version == 1)
            {
                privateTransactionStatus = DeserializePrivateTransactionStatus(ref reader);
            }

            // Construct domain object.
            try
            {
                return version switch
                {
                    0 => new CreateManagedPropertyV0(
                        sender,
                        receiver,
                        name,
                        category,
                        subcategory,
                        website,
                        description,
                        type,
                        tokenType,
                        previousId),
                    1 => new CreateManagedPropertyV1(
                        sender,
                        receiver,
                        name,
                        category,
                        subcategory,
                        website,
                        description,
                        type,
                        tokenType,
                        privateTransactionStatus!.Value,
                        previousId),
                    _ => throw new ArgumentOutOfRangeException(nameof(version)),
                };
            }
            catch (ArgumentException ex) when (ex.ParamName == "name")
            {
                throw new TransactionSerializationException("Invalid name.", ex);
            }
        }

        public override void Serialize(IBufferWriter<byte> writer, Elysium.Transaction transaction)
        {
            var tx = transaction as CreateManagedPropertyV0;

            if (tx == null)
            {
                throw new ArgumentException("The transaction is not supported.", nameof(transaction));
            }

            SerializePropertyType(writer, tx.Type);
            SerializeTokenType(writer, tx.TokenType);
            SerializePropertyId(writer, tx.PreviousId);
            SerializeString(writer, tx.Category);
            SerializeString(writer, tx.Subcategory);
            SerializeString(writer, tx.Name);
            SerializeString(writer, tx.Website);
            SerializeString(writer, tx.Description);

            if (tx is CreateManagedPropertyV1 v1)
            {
                SerializePrivateTransactionStatus(writer, v1.PrivateTransactionStatus);
            }
        }
    }
}
