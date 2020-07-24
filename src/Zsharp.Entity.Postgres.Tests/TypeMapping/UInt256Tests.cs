namespace Zsharp.Entity.Postgres.Tests.TypeMapping
{
    using NBitcoin;
    using Xunit;
    using Zsharp.Entity.Postgres.TypeMapping;

    public sealed class UInt256Tests
    {
        readonly UInt256 subject;

        public UInt256Tests()
        {
            this.subject = new UInt256();
        }

        [Fact]
        public void GenerateSqlLiteral_WithNonNull_ShouldReturnCorrectPostgresLiteral()
        {
            var domain = uint256.Parse("65b19f3341f4430a8a273052a588fafddd15f5ae11bbd93791a40ece6a0ac23f");

            var result = this.subject.GenerateSqlLiteral(domain);

            Assert.Equal(@"'\x65b19f3341f4430a8a273052a588fafddd15f5ae11bbd93791a40ece6a0ac23f'", result);
        }
    }
}
