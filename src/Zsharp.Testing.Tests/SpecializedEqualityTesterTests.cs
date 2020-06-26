namespace Zsharp.Testing.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using Xunit;

    public sealed class SpecializedEqualityTesterTests
    {
        readonly FakeComparand comparand;
        readonly FakeComparand comparator1;
        readonly FakeComparand comparator2;
        readonly SpecializedEqualityTester<FakeComparand> subject;

        public SpecializedEqualityTesterTests()
        {
            this.comparand = new FakeComparand();
            this.comparator1 = new FakeComparand();
            this.comparator2 = new FakeComparand();
            this.subject = new SpecializedEqualityTester<FakeComparand>(
                this.comparand,
                s => this.comparator1,
                s => this.comparator2);
        }

        [Fact]
        public void Compare_WhenEnumerate_ShouldInvokeSpecializedEquals()
        {
            // Act.
            this.subject.ToList();

            // Assert.
            this.comparand.StubbedEquals.Verify(e => e(this.comparator1), Times.Once());
            this.comparand.StubbedEquals.Verify(e => e(this.comparator2), Times.Once());
        }

        sealed class FakeComparand : IEquatable<FakeComparand>
        {
            public FakeComparand()
            {
                this.StubbedEquals = new Mock<Func<object?, bool>>();
            }

            public Mock<Func<object?, bool>> StubbedEquals { get; }

            public bool Equals(FakeComparand? other) => this.StubbedEquals.Object(other);
        }
    }
}
