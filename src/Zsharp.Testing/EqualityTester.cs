namespace Zsharp.Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public abstract class EqualityTester<TComparand, TComparator> : IEnumerable<bool>
    {
        readonly IEnumerable<Func<TComparand, TComparator>> comparators;

        protected EqualityTester(TComparand comparand, IEnumerable<Func<TComparand, TComparator>> comparators)
        {
            if (comparand == null)
            {
                throw new ArgumentNullException(nameof(comparand));
            }

            this.comparators = comparators;
            this.Comparand = comparand;
        }

        [NotNull]
        public TComparand Comparand { get; }

        public IEnumerator<bool> GetEnumerator()
        {
            foreach (var comparator in this.comparators)
            {
                yield return this.Compare(comparator(this.Comparand));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected abstract bool Compare(TComparator comparator);
    }
}
