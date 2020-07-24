namespace Zsharp.Entity.Tests
{
    using NBitcoin;
    using Xunit;

    public sealed class ConvertersTests
    {
        [Fact]
        public void ScriptToBytes_ToProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var value = new Script(
                "OP_DUP OP_HASH160 dfb52dff01bf04f983d6255e2ab9ff4084dd7517 OP_EQUALVERIFY OP_CHECKSIG");

            var converted = (byte[])Converters.ScriptToBytes.ConvertToProvider(value);

            Assert.Equal(value.ToBytes(), converted);
        }

        [Fact]
        public void ScriptToBytes_FromProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var value = new Script(
                "OP_DUP OP_HASH160 dfb52dff01bf04f983d6255e2ab9ff4084dd7517 OP_EQUALVERIFY OP_CHECKSIG");

            var converted = (Script)Converters.ScriptToBytes.ConvertFromProvider(value.ToBytes());

            Assert.Equal(value, converted);
        }

        [Fact]
        public void TargetToInt32_ToProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var converted = (int)Converters.TargetToInt32.ConvertToProvider(Target.Difficulty1);

            Assert.Equal(Target.Difficulty1.ToCompact(), (uint)converted);
        }

        [Fact]
        public void TargetToInt32_FromProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var converted = (Target)Converters.TargetToInt32.ConvertFromProvider((int)Target.Difficulty1.ToCompact());

            Assert.Equal(Target.Difficulty1, converted);
        }

        [Fact]
        public void UInt256ToBytes_ToProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var converted = (byte[])Converters.UInt256ToBytes.ConvertToProvider(uint256.One);

            Assert.Equal(uint256.One.ToBytes(false), converted);
        }

        [Fact]
        public void UInt256ToBytes_FromProviderWithCorrectInput_ShouldReturnExpectedValue()
        {
            var converted = (uint256)Converters.UInt256ToBytes.ConvertFromProvider(uint256.One.ToBytes(false));

            Assert.Equal(uint256.One, converted);
        }
    }
}
