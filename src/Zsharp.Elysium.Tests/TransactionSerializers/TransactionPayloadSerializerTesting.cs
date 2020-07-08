namespace Zsharp.Elysium.Tests.TransactionSerializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        protected abstract IEnumerable<(int Version, byte[] Payload)> CreateIncompleteData();

        protected abstract IEnumerable<(int Version, byte[] Payload, string ExpectedError)> CreateInvalidData();

        protected abstract IEnumerable<(Elysium.Transaction Transaction, byte[] Expected)> CreateSupportedVersions();

        protected abstract Elysium.Transaction CreateUnsupportedVersion();

        protected abstract IEnumerable<(int Version, byte[] Payload, Action<Elysium.Transaction> Assert)> CreateValidData();

        [Fact]
        public void TransactionId_GetValue_ShouldReturnCorrectValue()
        {
            Assert.Equal(this.TransactionId, this.Subject.TransactionId);
        }

        [Fact]
        public void Deserialize_WithInvalidVersion_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                "version",
                () => this.Subject.Deserialize(
                    this.ValidSenders.First(),
                    this.ValidReceivers.First(),
                    new byte[0],
                    this.InvalidVersion));
        }

        [Fact]
        public void Deserialize_WithIncompleteData_ShouldThrow()
        {
            foreach (var data in this.CreateIncompleteData())
            {
                Assert.Throws<ArgumentException>(
                    "data",
                    () => this.Subject.Deserialize(
                        this.ValidSenders.First(),
                        this.ValidReceivers.First(),
                        data.Payload,
                        data.Version));
            }
        }

        [Fact]
        public void Deserialize_WithInvalidData_ShouldThrow()
        {
            foreach (var data in this.CreateInvalidData())
            {
                // Act.
                var ex = Assert.Throws<TransactionSerializationException>(
                    () => this.Subject.Deserialize(
                        this.ValidSenders.First(),
                        this.ValidReceivers.First(),
                        data.Payload,
                        data.Version));

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
                var result = this.Subject.Deserialize(sender, receiver, data.Payload, data.Version);

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

            using (var output = new MemoryStream())
            {
                Assert.Throws<ArgumentException>("transaction", () => this.Subject.Serialize(output, tx));
            }
        }

        [Fact]
        public void Serialize_WithUnsupportedVersion_ShouldThrow()
        {
            var tx = this.CreateUnsupportedVersion();

            using (var output = new MemoryStream())
            {
                Assert.Throws<ArgumentException>("transaction", () => this.Subject.Serialize(output, tx));
            }
        }

        [Fact]
        public void Serialize_WithSupportedVersion_ShouldSerializeCorrectly()
        {
            foreach (var data in this.CreateSupportedVersions())
            using (var output = new MemoryStream())
            {
                this.Subject.Serialize(output, data.Transaction);

                Assert.Equal(data.Expected, output.ToArray());
            }
        }
    }
}
