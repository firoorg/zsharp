namespace Zsharp.LightweightIndexer.Tests
{
    using System;
    using Xunit;
    using Zsharp.Testing;

    public sealed class BlockEventArgsTests
    {
        readonly BlockEventArgs subject;

        public BlockEventArgsTests()
        {
            this.subject = new BlockEventArgs(TestBlock.Regtest3, 3);
        }

        [Fact]
        public void Constructor_WithNegativeHeight_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("height", () => new BlockEventArgs(TestBlock.Regtest0, -1));
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Equal(TestBlock.Regtest3, this.subject.Block);
            Assert.Equal(3, this.subject.Height);
        }
    }
}
