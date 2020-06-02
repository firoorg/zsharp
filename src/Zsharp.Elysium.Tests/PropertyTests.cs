using Xunit;

namespace Zsharp.Elysium.Tests
{
    public sealed class PropertyTests
    {
        readonly Property subject;

        public PropertyTests()
        {
            this.subject = new Property(new PropertyId(1), TokenType.Divisible);
        }

        [Fact]
        public void Constructor_WhenSuccess_ShouldInitProperties()
        {
            Assert.Equal(this.subject.Id, new PropertyId(1));
            Assert.Equal(TokenType.Divisible, this.subject.Type);
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
            var other = new Property(new PropertyId(2), this.subject.Type);

            Assert.False(this.subject.Equals(other));
        }

        [Fact]
        public void Equals_WithSameId_ShouldReturnTrue()
        {
            var other = new Property(new PropertyId(1), TokenType.Indivisible);

            Assert.True(this.subject.Equals(other));
        }
    }
}
