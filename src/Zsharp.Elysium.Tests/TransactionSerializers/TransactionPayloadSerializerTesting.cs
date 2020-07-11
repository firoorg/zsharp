namespace Zsharp.Elysium.Tests.TransactionSerializers
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public abstract class TransactionPayloadSerializerTesting<T> where T : TransactionPayloadSerializer
    {
        protected TransactionPayloadSerializerTesting()
        {
        }

        protected virtual int InvalidVersion => 99;

        protected abstract T Subject { get; }

        protected abstract int TransactionId { get; }

        protected virtual IEnumerable<BitcoinAddress?> ValidReceivers { get; } = new[] { TestAddress.Regtest2 };

        protected virtual IEnumerable<BitcoinAddress?> ValidSenders { get; } = new[] { TestAddress.Regtest1 };

        protected abstract IEnumerable<(int Version, byte[] Payload, string ExpectedError)> CreateInvalidData();

        protected abstract IEnumerable<(Elysium.Transaction Transaction, byte[] Expected)> CreateSupportedVersions();

        protected abstract IEnumerable<(int Version, byte[] Payload, Action<Elysium.Transaction> Assert)> CreateValidData();

        [Fact]
        public void TransactionId_GetValue_ShouldReturnCorrectValue()
        {
            Assert.Equal(this.TransactionId, this.Subject.TransactionId);
        }

        [Fact]
        public void Deserialize_WithInvalidVersion_ShouldThrow()
        {
            var payload = this.CreateValidData().First().Payload;

            Assert.Throws<ArgumentOutOfRangeException>(
                "version",
                () =>
                {
                    var sender = this.ValidSenders.First();
                    var receiver = this.ValidReceivers.First();
                    var reader = SerializationTesting.CreateReader(payload);

                    this.Subject.Deserialize(sender, receiver, ref reader, this.InvalidVersion);
                });
        }

        [Fact]
        public void Deserialize_WithInvalidData_ShouldThrow()
        {
            foreach (var data in this.CreateInvalidData())
            {
                // Act.
                var ex = Assert.Throws<TransactionSerializationException>(
                    () =>
                    {
                        var sender = this.ValidSenders.First();
                        var receiver = this.ValidReceivers.First();
                        var reader = SerializationTesting.CreateReader(data.Payload);

                        this.Subject.Deserialize(sender, receiver, ref reader, data.Version);
                    });

                // Assert.
                Assert.Equal(data.ExpectedError, ex.Message);
            }
        }

        [Fact]
        public void Deserialize_WithValidData_ShouldDeserializeCorrectly()
        {
            foreach (var data in this.CreateValidData())
            foreach (var sender in this.ValidSenders)
            foreach (var receiver in this.ValidReceivers)
            {
                var reader = SerializationTesting.CreateReader(data.Payload);
                var result = this.Subject.Deserialize(sender, receiver, ref reader, data.Version);

                Assert.Equal(0, reader.Remaining);
                Assert.Equal(this.TransactionId, result.Id);
                Assert.Equal(receiver, result.Receiver);
                Assert.Equal(sender, result.Sender);
                Assert.Equal(data.Version, result.Version);

                data.Assert(result);
            }
        }

        [Fact]
        public void Serialize_WithOtherTransaction_ShouldThrow()
        {
            var tx = new FakeTransaction(null, null);
            var writer = new ArrayBufferWriter<byte>();

            Assert.Throws<ArgumentException>("transaction", () => this.Subject.Serialize(writer, tx));
        }

        [Fact]
        public void Serialize_WithSupportedVersion_ShouldSerializeCorrectly()
        {
            foreach (var data in this.CreateSupportedVersions())
            {
                var writer = new ArrayBufferWriter<byte>();

                this.Subject.Serialize(writer, data.Transaction);

                Assert.Equal(data.Expected, writer.WrittenSpan.ToArray());
            }
        }
    }
}
