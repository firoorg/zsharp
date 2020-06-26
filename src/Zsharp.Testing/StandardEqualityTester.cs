namespace Zsharp.Testing
{
    using System;

    public sealed class StandardEqualityTester<T> : EqualityTester<T, object?>
    {
        public StandardEqualityTester(T comparand, params Func<T, object?>[] comparators)
            : base(comparand, comparators)
        {
        }

        protected override bool Compare(object? comparator)
        {
            return this.Comparand.Equals(comparator);
        }
    }
}
