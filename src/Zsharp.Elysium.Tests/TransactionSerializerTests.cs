namespace Zsharp.Elysium.Tests
{
    using System;
    using System.IO;
    using Moq;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class TransactionSerializerTests
    {
        readonly FakeTransactionPayloadSerializer payload1, payload2;
        readonly TransactionSerializer subject;

        public TransactionSerializerTests()
        {
            this.payload1 = new FakeTransactionPayloadSerializer(0);
            this.payload2 = new FakeTransactionPayloadSerializer(1);
            this.subject = new TransactionSerializer(new[] { this.payload1, this.payload2 });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Deserialize_WithNotEnoughData_ShouldThrow(int length)
        {
            var data = new byte[length];

            var ex = Assert.Throws<TransactionSerializationException>(() => this.subject.Deserialize(null, null, data));

            Assert.Equal("Not enough data.", ex.Message);
        }

        [Fact]
        public void Deserialize_WithUpsupportedTransaction_ShouldThrow()
        {
            // Arrange.
            byte[] data;

            using (var stream = RawTransaction.Create(2, 0))
            {
                data = stream.ToArray();
            }

            // Act.
            var ex = Assert.Throws<TransactionSerializationException>(() => this.subject.Deserialize(null, null, data));

            // Assert.
            Assert.Equal("Unknow transaction.", ex.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1, (byte)0x00)]
        [InlineData(1, (byte)0x00, (byte)0x01)]
        public void Deserialize_WithSupportedTransaction_ShouldInvokePayloadSerializer(int id, params byte[] payload)
        {
            // Arrange.
            var tx = new FakeTransaction(
                TestAddress.Regtest1,
                TestAddress.Regtest2,
                id,
                Elysium.Transaction.MaxVersion);
            byte[] data;

            using (var stream = RawTransaction.Create(tx.Id, tx.Version))
            {
                stream.Write(payload);
                data = stream.ToArray();
            }

            this.payload2.StubbedDeserialize
                .Setup(f => f(tx.Sender, tx.Receiver, payload, tx.Version))
                .Returns(tx);

            // Act.
            var result = this.subject.Deserialize(TestAddress.Regtest1, TestAddress.Regtest2, data);

            // Assert.
            Assert.Same(tx, result);

            this.payload1.StubbedDeserialize.Verify(
                f => f(It.IsAny<BitcoinAddress?>(), It.IsAny<BitcoinAddress?>(), It.IsAny<byte[]>(), It.IsAny<int>()),
                Times.Never());

            this.payload2.StubbedDeserialize.Verify(
                f => f(TestAddress.Regtest1, TestAddress.Regtest2, payload, Elysium.Transaction.MaxVersion),
                Times.Once());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(Int32.MaxValue)]
        public void Serialize_WithUnsupportedTransaction_ShouldThrow(int id)
        {
            var tx = new FakeTransaction(null, null, id, 0);

            Assert.Throws<ArgumentException>("transaction", () => this.subject.Serialize(tx));
        }

        [Theory]
        [InlineData(1, 0, (byte)0x00, (byte)0x01, (byte)0x00, (byte)0x00)]
        [InlineData(0, 0, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(1, 1, (byte)0x00, (byte)0x01, (byte)0x00, (byte)0x01)]
        [InlineData(0, 1, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(Elysium.Transaction.MaxVersion, 0, (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x00, (byte)0x10)]
        [InlineData(Elysium.Transaction.MaxVersion, 1, (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x01, (byte)0x10)]
        public void Serialize_WithSupportedTransaction_ShouldReturnCorrectData(
            int version,
            int id,
            params byte[] expected)
        {
            // Arrange.
            var tx = new FakeTransaction(null, null, id, version);

            var serializer = id switch
            {
                0 => this.payload1,
                1 => this.payload2,
                _ => throw new ArgumentOutOfRangeException(nameof(id))
            };

            serializer.StubbedSerialize
                .Setup(f => f(It.IsAny<MemoryStream>(), It.IsAny<Elysium.Transaction>()))
                .Callback((MemoryStream output, Elysium.Transaction tx) =>
                {
                    output.Write(expected, 4, expected.Length - 4);
                });

            // Act.
            var result = this.subject.Serialize(tx);

            // Assert.
            Assert.Equal(expected, result);

            serializer.StubbedSerialize.Verify(
                f => f(It.IsNotNull<MemoryStream>(), tx),
                Times.Once());
        }
    }
}
