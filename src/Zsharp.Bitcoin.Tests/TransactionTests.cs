namespace Zsharp.Bitcoin.Tests
{
    using Moq;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class TransactionTests
    {
        readonly ConsensusFactory factory;
        readonly Transaction subject;

        public TransactionTests()
        {
            this.factory = Networks.Default.Mainnet.Consensus.ConsensusFactory;
            this.subject = this.factory.CreateTransaction();
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Null(this.subject.GetElysiumTransaction());
        }

        [Fact]
        public void ElysiumTransaction_WhenAssigned_ShouldUpdated()
        {
            var value = new Mock<Elysium.Transaction>(null, null);

            this.subject.SetElysiumTransaction(value.Object);

            Assert.Same(value.Object, this.subject.GetElysiumTransaction());
        }

        [Fact]
        public void IsCoinBase_WithCoinBase_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetCoinBase1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithNormal_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetNormal1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithZecoinSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithSigmaSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithZerocoinRemint_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsCoinBase);
        }

        [Fact]
        public void IsSigmaSpend_WithSigmaSpend_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetSigmaSpend1.IsSigmaSpend());
        }

        [Fact]
        public void IsSigmaSpend_WithNonSigmaSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsSigmaSpend());
            Assert.False(TestTransaction.MainnetNormal1.IsSigmaSpend());
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsSigmaSpend());
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsSigmaSpend());
        }

        [Fact]
        public void IsZerocoinRemint_WithZerocoinRemint_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.RegtestZerocoinRemint1.IsZerocoinRemint());
        }

        [Fact]
        public void IsZerocoinRemint_WithNonZerocoinRemint_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetNormal1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsZerocoinRemint());
        }

        [Fact]
        public void IsZerocoinSpend_WithZerocoinSpend_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetZerocoinSpend1.IsZerocoinSpend());
        }

        [Fact]
        public void IsZerocoinSpend_WithNonZerocoinSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsZerocoinSpend());
            Assert.False(TestTransaction.MainnetNormal1.IsZerocoinSpend());
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsZerocoinSpend());
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsZerocoinSpend());
        }

        [Fact]
        public void GetConsensusFactory_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var result = this.subject.GetConsensusFactory();

            Assert.Same(this.factory, result);
        }
    }
}
