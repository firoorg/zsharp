namespace Zsharp.Testing
{
    using System.Collections;
    using System.Collections.Generic;

    public sealed class EnumerableAdapter : IEnumerable<object>
    {
        readonly IEnumerable enumerable;

        public EnumerableAdapter(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var item in this.enumerable)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
