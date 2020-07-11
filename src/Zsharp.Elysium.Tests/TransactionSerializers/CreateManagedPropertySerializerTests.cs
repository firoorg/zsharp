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

    public sealed class CreateManagedPropertySerializerTests :
        TransactionPayloadSerializerTesting<CreateManagedPropertySerializer>
    {
        protected override CreateManagedPropertySerializer Subject { get; } = new CreateManagedPropertySerializer();

        protected override int TransactionId => CreateManagedPropertyV0.StaticId;

        protected override IEnumerable<BitcoinAddress?> ValidReceivers { get; } = new[] { null, TestAddress.Regtest2 };

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
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x00, 0x61, 0x00, 0x21, 0x00 }, "Invalid name."),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x00, 0x61, 0x00, 0x21, 0x00, 0x00 }, "Invalid name."),
            };
        }

        protected override IEnumerable<(Elysium.Transaction Transaction, byte[] Expected)> CreateSupportedVersions()
        {
            return new (Elysium.Transaction, byte[])[]
            {
                (new CreateManagedPropertyV0(TestAddress.Regtest1, null, "BC", "", "A", "ab", "!@", PropertyType.Production, TokenType.Divisible, null), new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }),
                (new CreateManagedPropertyV0(TestAddress.Regtest1, null, "BC", "", "A", "ab", "!@", PropertyType.Production, TokenType.Divisible, new PropertyId(10)), new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }),
                (new CreateManagedPropertyV1(TestAddress.Regtest1, null, "BC", "", "A", "ab", "!@", PropertyType.Production, TokenType.Divisible, PrivateTransactionStatus.SoftDisabled, null), new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }),
                (new CreateManagedPropertyV1(TestAddress.Regtest1, null, "BC", "", "A", "ab", "!@", PropertyType.Production, TokenType.Divisible, PrivateTransactionStatus.HardDisabled, new PropertyId(10)), new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x02 }),
            };
        }

        protected override IEnumerable<(int Version, byte[] Payload, Action<Elysium.Transaction> Assert)> CreateValidData()
        {
            return new (int, byte[], Action<Elysium.Transaction>)[]
            {
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Test, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Indivisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.NotNull(tx.PreviousId);
                    Assert.Equal(1, tx.PreviousId!.Value);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x21, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Equal("!", tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("B", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("B", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x62, 0x00, 0x21, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("b", tx.Website);
                    Assert.Equal("!@", tx.Description);
                }),
                (0, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x40, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV0>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("@", tx.Description);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Test, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Indivisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.NotNull(tx.PreviousId);
                    Assert.Equal(1, tx.PreviousId!.Value);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x21, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Equal("!", tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("B", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x00, 0x61, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("B", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x62, 0x00, 0x21, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("b", tx.Website);
                    Assert.Equal("!@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x40, 0x00, 0x00 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftDisabled, tx.PrivateTransactionStatus);
                }),
                (1, new byte[] { 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41, 0x00, 0x42, 0x43, 0x00, 0x61, 0x62, 0x00, 0x40, 0x00, 0x01 }, result =>
                {
                    var tx = Assert.IsType<CreateManagedPropertyV1>(result);

                    Assert.Equal(PropertyType.Production, tx.Type);
                    Assert.Equal(TokenType.Divisible, tx.TokenType);
                    Assert.Null(tx.PreviousId);
                    Assert.Empty(tx.Category);
                    Assert.Equal("A", tx.Subcategory);
                    Assert.Equal("BC", tx.Name);
                    Assert.Equal("ab", tx.Website);
                    Assert.Equal("@", tx.Description);
                    Assert.Equal(PrivateTransactionStatus.SoftEnabled, tx.PrivateTransactionStatus);
                }),
            };
        }
    }
}
