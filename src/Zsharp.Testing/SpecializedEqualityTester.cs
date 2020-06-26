namespace Zsharp.Testing
{
    using System;

    public sealed class SpecializedEqualityTester<T> : EqualityTester<T, T>
        where T : IEquatable<T>
    {
        public SpecializedEqualityTester(T comparand, params Func<T, T>[] comparators)
            : base(comparand, comparators)
        {
        }

        protected override bool Compare(T comparator)
        {
            return this.Comparand.Equals(comparator);
        }
    }
}
