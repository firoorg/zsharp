namespace Zsharp.Elysium.Tests.TransactionSerializers
{
    using System;
    using System.Collections.Generic;
    using NBitcoin;
    using Xunit;
    using Zsharp.Elysium;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Elysium.TransactionSerializers;
    using Zsharp.Testing;

    public sealed class GrantTokensSerializerTests : TransactionPayloadSerializerTesting<GrantTokensSerializer>
    {
        protected override GrantTokensSerializer Subject { get; } = new GrantTokensSerializer();

        protected override int TransactionId => GrantTokensV0.StaticId;

        protected override IEnumerable<BitcoinAddress?> ValidReceivers { get; } = new[]
        {
            null,
            TestAddress.Regtest2
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

        protected override IEnumerable<(int Version, byte[] Payload, string ExpectedError)> CreateInvalidData()
        {
            return new[]
            {
                (0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, "Invalid property identifier."),
                (0, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, "Invalid amount."),
            };
        }

        protected override IEnumerable<(Elysium.Transaction Transaction, byte[] Expected)> CreateSupportedVersions()
        {
            return new (Elysium.Transaction, byte[])[]
            {
                (new GrantTokensV0(TestAddress.Regtest1, TestAddress.Regtest2, new PropertyId(1), new TokenAmount(10)), new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A })
            };
        }

        protected override IEnumerable<(int Version, byte[] Payload, Action<Elysium.Transaction> Assert)> CreateValidData()
        {
            return new (int, byte[], Action<Elysium.Transaction>)[]
            {
                (0, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A }, result =>
                {
                    var tx = Assert.IsType<GrantTokensV0>(result);

                    Assert.Equal(10, tx.Amount.Value);
                    Assert.Equal(1, tx.Property.Value);
                })
            };
        }
    }
}
