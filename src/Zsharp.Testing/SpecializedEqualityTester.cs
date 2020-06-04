namespace Zsharp.Testing
{
    using System;
    using System.Collections.Generic;

    public sealed class SpecializedEqualityTester<T> : EqualityTester<T, T>
        where T : IEquatable<T>
    {
        readonly IEquatable<T> comparand;

        public SpecializedEqualityTester(T comparand, IEnumerable<Func<T, T>> comparators)
            : base(comparand, comparators)
        {
            this.comparand = comparand;
        }

        protected override bool Compare(T comparator)
        {
            return this.comparand.Equals(comparator);
        }
    }
}
