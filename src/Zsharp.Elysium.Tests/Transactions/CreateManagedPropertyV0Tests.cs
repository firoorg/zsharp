namespace Zsharp.Elysium.Tests.Transactions
{
    using System;
    using Xunit;
    using Zsharp.Elysium.Transactions;
    using Zsharp.Testing;

    public sealed class CreateManagedPropertyV0Tests
    {
        [Fact]
        public void Constructor_WithEmptyName_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(
                "name",
                () => new CreateManagedPropertyV0(
                    TestAddress.Regtest1,
                    null,
                    "",
                    "",
                    "",
                    "",
                    "",
                    PropertyType.Production,
                    TokenType.Divisible,
                    null));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            var tx = new CreateManagedPropertyV0(
                TestAddress.Regtest1,
                TestAddress.Regtest2,
                "abc",
                "def",
                "ghi",
                "jkf",
                "lmn",
                PropertyType.Test,
                TokenType.Indivisible,
                new PropertyId(10));

            Assert.Equal("def", tx.Category);
            Assert.Equal("lmn", tx.Description);
            Assert.Equal(CreateManagedPropertyV0.StaticId, tx.Id);
            Assert.Equal("abc", tx.Name);
            Assert.NotNull(tx.PreviousId);
            Assert.Equal(10, tx.PreviousId!.Value);
            Assert.Equal(TestAddress.Regtest2, tx.Receiver);
            Assert.Equal(TestAddress.Regtest1, tx.Sender);
            Assert.Equal("ghi", tx.Subcategory);
            Assert.Equal(TokenType.Indivisible, tx.TokenType);
            Assert.Equal(PropertyType.Test, tx.Type);
            Assert.Equal(0, tx.Version);
            Assert.Equal("jkf", tx.Website);
        }
    }
}
