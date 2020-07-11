namespace Zsharp.Elysium.Tests
{
    using System;
    using System.Buffers;
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
        public void Deserialize_NotEnoughData_ShouldThrow(int length)
        {
            var data = new byte[length];

            Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    this.subject.Deserialize(null, null, ref reader);
                });
        }

        [Fact]
        public void Deserialize_WithUpsupportedTransaction_ShouldThrow()
        {
            // Arrange.
            var writer = SerializationTesting.CreateWriter(2, 0);

            // Act.
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(writer);
                    this.subject.Deserialize(null, null, ref reader);
                });

            // Assert.
            Assert.Equal("Unknow transaction.", ex.Message);
        }

        [Fact]
        public void Deserialize_WithUnsupportedVersion_ShouldThrow()
        {
            // Arrange.
            var writer = SerializationTesting.CreateWriter(0, 1);

            this.payload1.StubbedDeserialize
                .Setup(f => f(It.IsAny<BitcoinAddress?>(), It.IsAny<BitcoinAddress?>(), It.IsAny<byte[]>(), 1))
                .Throws(new ArgumentOutOfRangeException("version"));

            // Act.
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(writer);
                    this.subject.Deserialize(null, null, ref reader);
                });

            // Assert.
            Assert.Equal("Unknow version.", ex.Message);

            this.payload1.StubbedDeserialize.Verify(
                f => f(null, null, new byte[0], 1),
                Times.Once());
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
            var writer = SerializationTesting.CreateWriter(tx.Id, tx.Version);
            var (selected, other) = tx.Id switch
            {
                0 => (this.payload1, this.payload2),
                1 => (this.payload2, this.payload1),
                _ => throw new NotImplementedException()
            };

            writer.Write(payload);

            selected.StubbedDeserialize
                .Setup(f => f(tx.Sender, tx.Receiver, payload, tx.Version))
                .Returns(tx);

            // Act.
            var reader = SerializationTesting.CreateReader(writer);
            var result = this.subject.Deserialize(tx.Sender, tx.Receiver, ref reader);

            // Assert.
            Assert.Same(tx, result);
            Assert.Equal(0, reader.Remaining);

            selected.StubbedDeserialize.Verify(
                f => f(tx.Sender, tx.Receiver, payload, tx.Version),
                Times.Once());

            other.StubbedDeserialize.Verify(
                f => f(It.IsAny<BitcoinAddress?>(), It.IsAny<BitcoinAddress?>(), It.IsAny<byte[]>(), It.IsAny<int>()),
                Times.Never());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(Elysium.Transaction.MaxId)]
        public void Serialize_WithUnsupportedTransaction_ShouldThrow(int id)
        {
            var tx = new FakeTransaction(null, null, id, 0);
            var writer = new ArrayBufferWriter<byte>();

            Assert.Throws<ArgumentException>("transaction", () => this.subject.Serialize(writer, tx));
        }

        [Theory]
        [InlineData(0, 0, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(1, 0, (byte)0x00, (byte)0x01, (byte)0x00, (byte)0x00)]
        [InlineData(0, 1, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(1, 1, (byte)0x00, (byte)0x01, (byte)0x00, (byte)0x01)]
        [InlineData(Elysium.Transaction.MaxVersion, 0, (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x00, (byte)0xFF)]
        [InlineData(Elysium.Transaction.MaxVersion, 1, (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x01, (byte)0xFF)]
        [InlineData(0, Elysium.Transaction.MaxId, (byte)0x00, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0x00)]
        [InlineData(1, Elysium.Transaction.MaxId, (byte)0x00, (byte)0x01, (byte)0xFF, (byte)0xFF, (byte)0x00)]
        [InlineData(32768, 49280, (byte)0x80, (byte)0x00, (byte)0xC0, (byte)0x80, (byte)0x00)]
        public void Serialize_WithSupportedTransaction_ShouldInvokePayloadSerializer(
            int version,
            int id,
            params byte[] expected)
        {
            // Arrange.
            var tx = new FakeTransaction(null, null, id, version);
            var serializer = new FakeTransactionPayloadSerializer(tx.Id);
            var subject = new TransactionSerializer(new[] { serializer });
            var writer = new ArrayBufferWriter<byte>();

            serializer.StubbedSerialize
                .Setup(f => f(It.IsAny<IBufferWriter<byte>>(), It.IsAny<Elysium.Transaction>()))
                .Callback((IBufferWriter<byte> writer, Elysium.Transaction tx) =>
                {
                    writer.Write(new ReadOnlySpan<byte>(expected, 4, expected.Length - 4));
                });

            // Act.
            subject.Serialize(writer, tx);

            // Assert.
            Assert.Equal(expected, writer.WrittenSpan.ToArray());

            serializer.StubbedSerialize.Verify(f => f(writer, tx), Times.Once());
        }
    }
}
