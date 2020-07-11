namespace Zsharp.Elysium.Tests
{
    using System;
    using System.Buffers;
    using Xunit;
    using Zsharp.Elysium.Transactions;
    using Serializer = TransactionPayloadSerializer;

    public sealed class TransactionPayloadSerializerTests
    {
        [Fact]
        public void DeserializePrivateTransactionStatus_NotEnoughData_ShouldThrow()
        {
            var data = new byte[0];

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializePrivateTransactionStatus(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Theory]
        [InlineData((byte)4)]
        [InlineData((byte)255)]
        public void DeserializePrivateTransactionStatus_WithInvalidData_ShouldThrow(params byte[] data)
        {
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializePrivateTransactionStatus(ref reader);
                });

            Assert.Equal("Invalid private transaction status.", ex.Message);
        }

        [Theory]
        [InlineData(PrivateTransactionStatus.SoftDisabled, (byte)0)]
        [InlineData(PrivateTransactionStatus.SoftEnabled, (byte)1)]
        [InlineData(PrivateTransactionStatus.HardDisabled, (byte)2)]
        [InlineData(PrivateTransactionStatus.HardEnabled, (byte)3)]
        public void DeserializePrivateTransactionStatus_WithValidData_ShouldDeserializeCorrectly(
            PrivateTransactionStatus expected,
            params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializePrivateTransactionStatus(ref reader);

            Assert.Equal(expected, result);
            Assert.Equal(0, reader.Remaining);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void DeserializePropertyId_NotEnoughData_ShouldThrow(int size)
        {
            var data = new byte[size];

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializePropertyId(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Fact]
        public void DeserializePropertyId_WithZero_ShouldReturnNull()
        {
            var data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializePropertyId(ref reader);

            Assert.Null(result);
            Assert.Equal(0, reader.Remaining);
        }

        [Theory]
        [InlineData(1L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(16777215L, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(4294967295L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void DeserializePropertyId_WithNonZero_ShouldDeserializeCorrectly(long expected, params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializePropertyId(ref reader);

            Assert.NotNull(result);
            Assert.Equal(expected, result!.Value);
            Assert.Equal(0, reader.Remaining);
        }

        [Fact]
        public void DeserializePropertyType_NotEnoughData_ShouldThrow()
        {
            var data = new byte[0];

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializePropertyType(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)3)]
        [InlineData((byte)255)]
        public void DeserializePropertyType_WithInvalidData_ShouldThrow(params byte[] data)
        {
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializePropertyType(ref reader);
                });

            Assert.Equal("Invalid property type.", ex.Message);
        }

        [Theory]
        [InlineData(PropertyType.Production, (byte)1)]
        [InlineData(PropertyType.Test, (byte)2)]
        public void DeserializePropertyType_WithValidData_ShouldDeserializeCorrectly(
            PropertyType expected,
            params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializePropertyType(ref reader);

            Assert.Equal(expected, result);
            Assert.Equal(0, reader.Remaining);
        }

        [Fact]
        public void DeserializeString_NotEnoughtData_ShouldThrow()
        {
            var data = new byte[0];

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializeString(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Theory]
        [InlineData((byte)0x01)]
        [InlineData((byte)0xFF)]
        [InlineData((byte)0x02, (byte)0xFF)]
        public void DeserializeString_DataIsNotString_ShouldThrow(params byte[] data)
        {
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializeString(ref reader);
                });

            Assert.Equal("Invalid string.", ex.Message);
        }

        [Theory]
        [InlineData("!", (byte)0x21, (byte)0x00)]
        [InlineData("qweRtY", (byte)0x71, (byte)0x77, (byte)0x65, (byte)0x52, (byte)0x74, (byte)0x59, (byte)0x00)]
        [InlineData("ภาษาไทย", (byte)0xE0, (byte)0xB8, (byte)0xA0, (byte)0xE0, (byte)0xB8, (byte)0xB2, (byte)0xE0, (byte)0xB8, (byte)0xA9, (byte)0xE0, (byte)0xB8, (byte)0xB2, (byte)0xE0, (byte)0xB9, (byte)0x84, (byte)0xE0, (byte)0xB8, (byte)0x97, (byte)0xE0, (byte)0xB8, (byte)0xA2, (byte)0x00)]
        [InlineData("���", (byte)0x80, (byte)0xC0, (byte)0xC0, (byte)0x00)]
        public void DeserializeString_DataIsString_ShouldDeserializeCorrectly(string expected, params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializeString(ref reader);

            Assert.Equal(expected, result);
            Assert.Equal(0, reader.Remaining);
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

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializeTokenAmount(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Theory]
        [InlineData(0L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(255L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF)]
        [InlineData(1099511627775L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(-1L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void DeserializeTokenAmount_WithEnoughData_ShouldDeserializeCorrectly(long expected, params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializeTokenAmount(ref reader);

            Assert.Equal(expected, result.Value);
            Assert.Equal(0, reader.Remaining);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void DeserializeTokenType_NotEnoughData_ShouldThrow(int size)
        {
            var data = new byte[size];

            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializeTokenType(ref reader);
                });

            Assert.Equal("Incomplete data.", ex.Message);
        }

        [Theory]
        [InlineData((byte)0x00, (byte)0x00)]
        [InlineData((byte)0x00, (byte)0x03)]
        [InlineData((byte)0x00, (byte)0xFF)]
        [InlineData((byte)0xFF, (byte)0x00)]
        [InlineData((byte)0xFF, (byte)0xFF)]
        [InlineData((byte)0x01, (byte)0x00)]
        [InlineData((byte)0x02, (byte)0x00)]
        public void DeserializeTokenType_WithInvalidData_ShouldThrow(params byte[] data)
        {
            var ex = Assert.Throws<TransactionSerializationException>(
                () =>
                {
                    var reader = SerializationTesting.CreateReader(data);
                    Serializer.DeserializeTokenType(ref reader);
                });

            Assert.Equal("Invalid token type.", ex.Message);
        }

        [Theory]
        [InlineData(TokenType.Divisible, (byte)0x00, (byte)0x02)]
        [InlineData(TokenType.Indivisible, (byte)0x00, (byte)0x01)]
        public void DeserializeTokenType_WithValidData_ShouldDeserializeCorrectly(
            TokenType expected,
            params byte[] data)
        {
            var reader = SerializationTesting.CreateReader(data);

            var result = Serializer.DeserializeTokenType(ref reader);

            Assert.Equal(expected, result);
            Assert.Equal(0, reader.Remaining);
        }

        [Theory]
        [InlineData(PrivateTransactionStatus.HardDisabled, (byte)2)]
        [InlineData(PrivateTransactionStatus.HardEnabled, (byte)3)]
        [InlineData(PrivateTransactionStatus.SoftDisabled, (byte)0)]
        [InlineData(PrivateTransactionStatus.SoftEnabled, (byte)1)]
        public void SerializePrivateTransactionStatus_WithValidValue_ShouldSerializeCorrectly(
            PrivateTransactionStatus value,
            params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializePrivateTransactionStatus, value, expected);
        }

        [Fact]
        public void SerializePropertyId_WithNullValue_ShouldSerializeAsZero()
        {
            TestValueSerializer<PropertyId?>(
                Serializer.SerializePropertyId,
                null,
                new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        [Theory]
        [InlineData(1L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x01)]
        [InlineData(16777215L, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(4294967295L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void SerializePropertyId_WithNonNullValue_ShouldSerializeCorrectly(long value, params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializePropertyId, new PropertyId(value), expected);
        }

        [Theory]
        [InlineData(PropertyType.Production, (byte)1)]
        [InlineData(PropertyType.Test, (byte)2)]
        public void SerializePropertyType_WithValidValue_ShouldSerializeCorrectly(
            PropertyType value,
            params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializePropertyType, value, expected);
        }

        [Theory]
        [InlineData("!", (byte)0x21, (byte)0x00)]
        [InlineData("qweRtY", (byte)0x71, (byte)0x77, (byte)0x65, (byte)0x52, (byte)0x74, (byte)0x59, (byte)0x00)]
        [InlineData("ภาษาไทย", (byte)0xE0, (byte)0xB8, (byte)0xA0, (byte)0xE0, (byte)0xB8, (byte)0xB2, (byte)0xE0, (byte)0xB8, (byte)0xA9, (byte)0xE0, (byte)0xB8, (byte)0xB2, (byte)0xE0, (byte)0xB9, (byte)0x84, (byte)0xE0, (byte)0xB8, (byte)0x97, (byte)0xE0, (byte)0xB8, (byte)0xA2, (byte)0x00)]
        public void SerializeString_WithValidValue_ShouldSerializeCorrectly(string value, params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializeString, value, expected);
        }

        [Theory]
        [InlineData(0L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00)]
        [InlineData(255L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF)]
        [InlineData(1099511627775L, (byte)0x00, (byte)0x00, (byte)0x00, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        [InlineData(-1L, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF, (byte)0xFF)]
        public void SerializeTokenAmount_WithValidValue_ShouldSerializeCorrectly(long value, params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializeTokenAmount, new TokenAmount(value), expected);
        }

        [Theory]
        [InlineData(TokenType.Divisible, (byte)0, (byte)2)]
        [InlineData(TokenType.Indivisible, (byte)0, (byte)1)]
        public void SerializeTokenType_WithValidValue_ShouldSerializeCorrectly(TokenType value, params byte[] expected)
        {
            TestValueSerializer(Serializer.SerializeTokenType, value, expected);
        }

        static void TestValueSerializer<T>(Action<IBufferWriter<byte>, T> serializer, T value, byte[] expected)
        {
            var writer = new ArrayBufferWriter<byte>();

            serializer(writer, value);

            Assert.Equal(expected, writer.WrittenSpan.ToArray());
        }
    }
}
