namespace Zsharp.Elysium.Tests.TransactionSerializers
{
    using System;
    using System.Collections.Generic;
    using NBitcoin;
    using Xunit;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Elysium.TransactionSerializers;
    using Zsharp.Testing;

    public sealed class SimpleSendSerializerTests : TransactionPayloadSerializerTesting<SimpleSendSerializer>
    {
        protected override SimpleSendSerializer Subject { get; } = new SimpleSendSerializer();

        protected override int TransactionId => SimpleSendV0.StaticId;

        protected override IEnumerable<BitcoinAddress?> ValidReceivers { get; } = new[]
        {
            null,
            TestAddress.Regtest2
        };

        protected override IEnumerable<BitcoinAddress?> ValidSenders { get; } = new[]
        {
            TestAddress.Regtest1
        };

        [Fact]
        public void Deserialize_WithNullSender_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(
                "sender",
                () =>
                {
                    var reader = SerializationTesting.CreateReader(new byte[0]);
                    this.Subject.Deserialize(null, null, ref reader, 0);
                });
        }

        protected override IEnumerable<(int Version, byte[] Payload, string ExpectedError)> CreateInvalidData() => new[]
        {
            (0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, "Invalid property identifier."),
            (0, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "Invalid amount."),
            (0, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, "Invalid amount."),
            (0, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "Invalid amount."),
        };

        protected override IEnumerable<(Elysium.Transaction Transaction, byte[] Expected)> CreateSupportedVersions() => new (Elysium.Transaction, byte[])[]
        {
            (new SimpleSendV0(TestAddress.Regtest1, null, new PropertyId(PropertyId.MinValue), new TokenAmount(1)), new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }),
            (new SimpleSendV0(TestAddress.Regtest1, null, new PropertyId(PropertyId.MinValue), new TokenAmount(long.MaxValue)), new byte[] { 0x00, 0x00, 0x00, 0x01, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }),
            (new SimpleSendV0(TestAddress.Regtest1, null, new PropertyId(PropertyId.MaxValue), new TokenAmount(1)), new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }),
            (new SimpleSendV0(TestAddress.Regtest1, null, new PropertyId(PropertyId.MaxValue), new TokenAmount(long.MaxValue)), new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }),
        };

        protected override IEnumerable<(int Version, byte[] Payload, Action<Elysium.Transaction> Assert)> CreateValidData() => new (int, byte[], Action<Elysium.Transaction>)[]
        {
            (0, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, result =>
            {
                var tx = Assert.IsType<SimpleSendV0>(result);

                Assert.Equal(1, tx.Amount.Value);
                Assert.Equal(1, tx.Property.Value);
            }),
            (0, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, result =>
            {
                var tx = Assert.IsType<SimpleSendV0>(result);

                Assert.Equal(long.MaxValue, tx.Amount.Value);
                Assert.Equal(1, tx.Property.Value);
            }),
            (0, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, result =>
            {
                var tx = Assert.IsType<SimpleSendV0>(result);

                Assert.Equal(1, tx.Amount.Value);
                Assert.Equal(PropertyId.MaxValue, tx.Property.Value);
            }),
            (0, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, result =>
            {
                var tx = Assert.IsType<SimpleSendV0>(result);

                Assert.Equal(long.MaxValue, tx.Amount.Value);
                Assert.Equal(PropertyId.MaxValue, tx.Property.Value);
            }),
        };
    }
}
