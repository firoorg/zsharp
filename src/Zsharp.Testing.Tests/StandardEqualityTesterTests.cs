namespace Zsharp.Testing.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using Xunit;

    public sealed class StandardEqualityTesterTests
    {
        readonly FakeComparand comparand;
        readonly object comparator1;
        readonly StandardEqualityTester<FakeComparand> subject;

        public StandardEqualityTesterTests()
        {
            this.comparand = new FakeComparand();
            this.comparator1 = new object();
            this.subject = new StandardEqualityTester<FakeComparand>(this.comparand, s => this.comparator1, s => null);
        }

        [Fact]
        public void Compare_WhenEnumerate_ShouldInvokeEquals()
        {
            // Act.
            this.subject.ToList();

            // Assert.
            this.comparand.StubbedEquals.Verify(e => e(this.comparator1), Times.Once());
            this.comparand.StubbedEquals.Verify(e => e(null), Times.Once());
        }

        sealed class FakeComparand
        {
            public FakeComparand()
            {
                this.StubbedEquals = new Mock<Func<object?, bool>>();
            }

            public Mock<Func<object?, bool>> StubbedEquals { get; }

            public override bool Equals(object? obj) => this.StubbedEquals.Object(obj);

            public override int GetHashCode() => base.GetHashCode();
        }
    }
}
