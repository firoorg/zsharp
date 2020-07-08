namespace Zsharp.Elysium.TransactionSerializers
{
    using System;
    using System.Buffers;
    using System.IO;
    using NBitcoin;
    using Zsharp.Elysium.Transactions;

    public sealed class SimpleSendSerializer : TransactionPayloadSerializer
    {
        public override int TransactionId => SimpleSendV0.StaticId;

        public override Elysium.Transaction Deserialize(
            BitcoinAddress? sender,
            BitcoinAddress? receiver,
            ReadOnlySpan<byte> data,
            int version)
        {
            if (ReferenceEquals(sender, null))
            {
                throw new ArgumentNullException(nameof(sender));
            }

            // Deserialize payload.
            PropertyId property;
            TokenAmount amount;

            switch (version)
            {
                case 0:
                    property = DeserializePropertyId(data.Slice(0));
                    amount = DeserializeTokenAmount(data.Slice(4));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version));
            }

            // Construct domain object.
            try
            {
                switch (version)
                {
                    case 0:
                        return new SimpleSendV0(sender, receiver, property, amount);
                    default:
                        throw new NotImplementedException($"Version {version} does not implemented.");
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new TransactionSerializationException("Invalid amount.", ex);
            }
        }

        public override void Serialize(MemoryStream output, Elysium.Transaction transaction)
        {
            switch (transaction)
            {
                case SimpleSendV0 tx when tx.Version == 0:
                    using (var memory = MemoryPool<byte>.Shared.Rent(12))
                    {
                        var buffer = memory.Memory.Span.Slice(0, 12);
                        WriteV0(buffer, tx);
                        output.Write(buffer);
                    }

                    break;
                default:
                    throw new ArgumentException("The transaction is not supported.", nameof(transaction));
            }

            void WriteV0(Span<byte> dest, SimpleSendV0 tx)
            {
                SerializePropertyId(dest.Slice(0), tx.Property);
                SerializeTokenAmount(dest.Slice(4), tx.Amount);
            }
        }
    }
}
