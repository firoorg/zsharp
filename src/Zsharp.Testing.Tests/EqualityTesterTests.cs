namespace Zsharp.Testing.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using Xunit;

    public sealed class EqualityTesterTests
    {
        readonly Mock<Func<string?, object?>> comparator1;
        readonly Mock<Func<string?, object?>> comparator2;
        readonly FakeEqualityTester subject;

        public EqualityTesterTests()
        {
            this.comparator1 = new Mock<Func<string?, object?>>();
            this.comparator2 = new Mock<Func<string?, object?>>();
            this.subject = new FakeEqualityTester("abc", new[] { this.comparator1.Object, this.comparator2.Object });
        }

        [Fact]
        public void Constructor_WithNullComparand_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(
                "comparand",
                () => new FakeEqualityTester(null, Enumerable.Empty<Func<string?, object?>>()));
        }

        [Fact]
        public void Constructor_WhenSucceeded_PropertiesShouldInitialized()
        {
            Assert.Equal("abc", this.subject.Comparand);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetEnumerator_WhenEnumerate_ShouldInvokeCompareForEachComparator(bool generic)
        {
            // Arrange.
            var result1 = new object();
            var result2 = new object();

            this.comparator1
                .Setup(c => c(It.IsAny<string>()))
                .Returns(result1);

            this.comparator2
                .Setup(c => c(It.IsAny<string>()))
                .Returns(result2);

            this.subject.StubbedCompare
                .Setup(c => c(result1))
                .Returns(true);

            this.subject.StubbedCompare
                .Setup(c => c(result2))
                .Returns(false);

            // Act.
            if (generic)
            {
                Assert.Equal(new[] { true, false }, this.subject);
            }
            else
            {
                Assert.Equal(new object[] { true, false }, new EnumerableAdapter(this.subject));
            }

            // Assert.
            this.comparator1.Verify(c => c("abc"), Times.Once());
            this.comparator2.Verify(c => c("abc"), Times.Once());

            this.subject.StubbedCompare.Verify(c => c(result1), Times.Once());
            this.subject.StubbedCompare.Verify(c => c(result2), Times.Once());
        }
    }
}
