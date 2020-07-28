namespace Zsharp.Bitcoin.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using NBitcoin;
    using Xunit;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.subject = new ServiceCollection();
        }

        [Theory]
        [InlineData(NetworkType.Mainnet)]
        [InlineData(NetworkType.Regtest)]
        [InlineData(NetworkType.Testnet)]
        public void AddZcoin_WithValidArgs_AllRegisteredServicesShouldResolvedSuccessfully(NetworkType type)
        {
            // Act.
            this.subject.AddZcoin(type);

            // Assert.
            using var services = this.subject.BuildServiceProvider();
            var network = services.GetRequiredService<Network>();

            Assert.Equal(type, network.NetworkType);
            Assert.StartsWith("xzc-", network.Name);
        }
    }
}
