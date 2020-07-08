namespace Zsharp.Elysium.Tests
{
    using System;
    using Xunit;

    public sealed class TransactionPayloadSerializerTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void DeserializePropertyId_NotEnoughData_ShouldThrow(int size)
        {
            var data = new byte[size];

            Assert.Throws<ArgumentException>("data", () => TransactionPayloadSerializer.DeserializePropertyId(data));
        }

        [Fact]
        public void DeserializePropertyId_WithInvalidId_ShouldThrow()
        {
            var data = new byte[] { 0x00, 0x00, 0x00, 0x00 };

            var ex = Assert.Throws<TransactionSerializationException>(
                () => TransactionPayloadSerializer.DeserializePropertyId(data));

            Assert.Equal("Invalid property identifier.", ex.Message);
        }

        [Theory]
        [InlineData(1L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(16777215L, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(4294967295L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void DeserializePropertyId_WithValidData_ShouldDeserializeCorrectly(long expected, params byte[] data)
        {
            var result = TransactionPayloadSerializer.DeserializePropertyId(data);

            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void DeserializeTokenAmount_NotEnoughData_ShouldThrow(int size)
        {
            var data = new byte[size];

            Assert.Throws<ArgumentException>("data", () => TransactionPayloadSerializer.DeserializeTokenAmount(data));
        }

        [Theory]
        [InlineData(0L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(255L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF)]
        [InlineData(1099511627775L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(-1L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void DeserializeTokenAmount_WithEnoughData_ShouldDeserializeCorrectly(long expected, params byte[] data)
        {
            var result = TransactionPayloadSerializer.DeserializeTokenAmount(data);

            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SerializePropertyId_NotEnoughSpace_ShouldThrow(int size)
        {
            var space = new byte[size];

            Assert.Throws<ArgumentException>(
                "destination",
                () => TransactionPayloadSerializer.SerializePropertyId(space, new PropertyId(PropertyId.MinValue)));
        }

        [Theory]
        [InlineData(1L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(16777215L, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(4294967295L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void SerializePropertyId_WithEnoughSpace_ShouldSerializeValueAsBigEndian(long id, params byte[] expected)
        {
            var value = new PropertyId(id);
            var data = new byte[4];

            TransactionPayloadSerializer.SerializePropertyId(data, value);

            Assert.Equal(expected, data);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void SerializeTokenAmount_NotEnoughSpace_ShouldThrow(int size)
        {
            var space = new byte[size];

            Assert.Throws<ArgumentException>(
                "destination",
                () => TransactionPayloadSerializer.SerializeTokenAmount(space, TokenAmount.Zero));
        }

        [Theory]
        [InlineData(0L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(255L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF)]
        [InlineData(1099511627775L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(-1L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void SerializeTokenAmount_WithEnoughSpace_ShouldSerializeValueAsBigEndian(long amount, params byte[] expected)
        {
            var value = new TokenAmount(amount);
            var data = new byte[8];

            TransactionPayloadSerializer.SerializeTokenAmount(data, value);

            Assert.Equal(expected, data);
        }
    }
}
