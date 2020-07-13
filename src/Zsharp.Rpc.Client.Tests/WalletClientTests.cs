namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using Xunit;
    using Zsharp.Testing;

    public sealed class WalletClientTests : RpcClientTesting<WalletClient>
    {
        [Fact]
        public async Task GetNewAddressAsync_WhenInvoke_ShouldSuccess()
        {
            await this.Subject.GetNewAddressAsync();
        }

        [Fact]
        public Task TransferAsync_WithInvalidAmount_ShouldThrow()
        {
            return Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "amount",
                () => this.Subject.TransferAsync(TestAddress.Regtest1, Money.Zero, null, null, false));
        }

        [Fact]
        public async Task TransferAsync_WithValidArgs_ShouldTransferSuccessfully()
        {
            // Arrange.
            var receiver = await this.GenerateNewAddressAsync();

            this.Node.Generate(101);

            // Act.
            var tx = await this.Subject.TransferAsync(receiver, Money.Coins(1), null, null, false);
            var hash = this.Node.Generate(1).Single();

            // Assert.
            var block = await this.GetBlockAsync(hash);

            Assert.Contains(block.Transactions, t => t.GetHash() == tx);
        }

        protected override WalletClient CreateSubject()
        {
            return new WalletClient(this.Factory, this.Client);
        }
    }
}
