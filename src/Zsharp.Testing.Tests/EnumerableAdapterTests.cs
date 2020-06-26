namespace Zsharp.Testing.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using Xunit;

    public sealed class EnumerableAdapterTests
    {
        readonly object item1;
        readonly object item2;
        readonly EnumerableAdapter subject;

        public EnumerableAdapterTests()
        {
            this.item1 = "abc";
            this.item2 = 3;
            this.subject = new EnumerableAdapter(new[] { this.item1, this.item2 });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetEnumerator_WhenEnumerate_ShouldReturnFromUnderlyingEnumerable(bool generic)
        {
            var enumerator = generic
                ? ((IEnumerable<object>)this.subject).GetEnumerator()
                : ((IEnumerable)this.subject).GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Same(this.item1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Same(this.item2, enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }
    }
}
