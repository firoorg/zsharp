namespace Zsharp.Elysium.Tests
{
    using System;
    using Xunit;
    using Zsharp.Testing;

    public sealed class PropertyTests
    {
        readonly Property subject;

        public PropertyTests()
        {
            this.subject = new Property(
                new PropertyId(1),
                "Token",
                "Foo",
                "Bar",
                "http://localhost",
                "Test.",
                TokenType.Divisible);
        }

        [Fact]
        public void Constructor_WhenSuccess_ShouldInitProperties()
        {
            Assert.Equal("Foo", this.subject.Category);
            Assert.Equal("Test.", this.subject.Description);
            Assert.Equal(this.subject.Id, new PropertyId(1));
            Assert.Equal("Token", this.subject.Name);
            Assert.Equal("Bar", this.subject.Subcategory);
            Assert.Equal(TokenType.Divisible, this.subject.Type);
            Assert.Equal("http://localhost", this.subject.WebsiteUrl);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            Assert.False(this.subject.Equals(null));
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            Assert.False(this.subject.Equals(this.subject.Id));
        }

        [Fact]
        public void Equals_WithDifferentId_ShouldReturnFalse()
        {
            var other = new Property(
                new PropertyId(2),
                this.subject.Name,
                this.subject.Category,
                this.subject.Subcategory,
                this.subject.WebsiteUrl,
                this.subject.Description,
                this.subject.Type);

            Assert.False(this.subject.Equals(other));
        }

        [Fact]
        public void Equals_WithSameId_ShouldReturnTrue()
        {
            var id = new PropertyId(1);
            var others = new Func<Property, Property>[]
            {
                l => new Property(id, l.Name, l.Category, l.Subcategory, l.WebsiteUrl, l.Description, l.Type),
                l => new Property(id, "abc", l.Category, l.Subcategory, l.WebsiteUrl, l.Description, l.Type),
                l => new Property(id, l.Name, "abc", l.Subcategory, l.WebsiteUrl, l.Description, l.Type),
                l => new Property(id, l.Name, l.Category, "abc", l.WebsiteUrl, l.Description, l.Type),
                l => new Property(id, l.Name, l.Category, l.Subcategory, "abc", l.Description, l.Type),
                l => new Property(id, l.Name, l.Category, l.Subcategory, l.WebsiteUrl, "abc", l.Type),
                l => new Property(
                    id,
                    l.Name,
                    l.Category,
                    l.Subcategory,
                    l.WebsiteUrl,
                    l.Description,
                    TokenType.Indivisible),
            };

            Assert.DoesNotContain(false, new StandardEqualityTester<Property>(this.subject, others));
        }
    }
}
