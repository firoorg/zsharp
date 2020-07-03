namespace Zsharp.Elysium.Tests
{
    using System;
    using Xunit;

    public sealed class TransactionSerializationExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_MessagePropertyShouldHaveTheSameValue()
        {
            var msg = "qwerty";

            var ex = new TransactionSerializationException(msg);

            Assert.Equal(msg, ex.Message);
        }

        [Fact]
        public void Constructor_WithInner_InnerExceptionPropertyShouldHaveTheSameValue()
        {
            var msg = "qwerty";
            var inner = new Exception();

            var ex = new TransactionSerializationException(msg, inner);

            Assert.Equal(msg, ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
