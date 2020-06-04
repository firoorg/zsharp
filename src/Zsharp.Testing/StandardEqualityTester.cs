namespace Zsharp.Testing
{
    using System;
    using System.Collections.Generic;

    public sealed class StandardEqualityTester<T> : EqualityTester<T, object>
    {
        public StandardEqualityTester(T comparand, IEnumerable<Func<T, object>> comparators)
            : base(comparand, comparators)
        {
        }

        protected override bool Compare(object comparator)
        {
            return this.Comparand.Equals(comparator);
        }
    }
}
