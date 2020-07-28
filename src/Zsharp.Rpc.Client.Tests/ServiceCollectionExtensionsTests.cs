namespace Zsharp.Rpc.Client.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Moq;
    using NBitcoin.RPC;
    using Xunit;
    using Zsharp.Bitcoin;
    using Zsharp.Elysium;

    public sealed class ServiceCollectionExtensionsTests
    {
        readonly Mock<ITransactionSerializer> serializer;
        readonly ServiceCollection subject;

        public ServiceCollectionExtensionsTests()
        {
            this.serializer = new Mock<ITransactionSerializer>();
            this.subject = new ServiceCollection();

            this.subject.AddSingleton(Networks.Default.Mainnet);
            this.subject.AddSingleton(this.serializer.Object);
        }

        [Fact]
        public void AddZcoinRpcClient_WithInvalidOptions_ShouldThrowWhenResolvingRegisteredServices()
        {
            // Act.
            this.subject.AddZcoinRpcClient(options =>
            {
                options.ServerUrl = null;
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IRpcClientFactory>());
        }

        [Fact]
        public void AddZcoinRpcClient_WithValidArgs_ShouldResolvedRegisteredServicesSuccessfully()
        {
            // Act.
            this.subject.AddZcoinRpcClient(options =>
            {
                options.Credential = new RPCCredentialString();
                options.ServerUrl = new Uri("http://localhost");
            });

            // Assert.
            using var services = this.subject.BuildServiceProvider();

            services.GetRequiredService<IRpcClientFactory>();
        }
    }
}
