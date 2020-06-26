namespace Zsharp.Testing.Tests
{
    using System;
    using System.Collections.Generic;
    using Moq;

    sealed class FakeEqualityTester : EqualityTester<string?, object?>
    {
        public FakeEqualityTester(string? comparand, IEnumerable<Func<string?, object?>> comparators)
            : base(comparand, comparators)
        {
            this.StubbedCompare = new Mock<Func<object?, bool>>();
        }

        public Mock<Func<object?, bool>> StubbedCompare { get; }

        protected override bool Compare(object? comparator)
        {
            return this.StubbedCompare.Object(comparator);
        }
    }
}
