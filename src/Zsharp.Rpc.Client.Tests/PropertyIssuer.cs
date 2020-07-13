namespace Zsharp.Rpc.Client.Tests
{
    using System.Threading.Tasks;
    using NBitcoin;
    using Zsharp.Elysium;

    sealed class PropertyIssuer
    {
        readonly RpcClientFactory factory;

        public PropertyIssuer(RpcClientFactory factory)
        {
            this.factory = factory;
            this.Type = PropertyType.Production;
            this.TokenType = TokenType.Indivisible;
            this.Category = "Company";
            this.Subcategory = "Private";
            this.Name = "Satang Corporation";
            this.Description = "Provides cryptocurrency solutions.";
            this.Url = "https://satang.com";
        }

        public string Category { get; set; }

        public Property? Current { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Subcategory { get; set; }

        public TokenType TokenType { get; set; }

        public PropertyType Type { get; set; }

        public string Url { get; set; }

        public async Task<string> IssueManagedAsync(BitcoinAddress owner)
        {
            await using (var client = await this.factory.CreatePropertyManagementClientAsync())
            {
                return await client.CreateManagedAsync(
                    owner,
                    this.Type,
                    this.TokenType,
                    this.Current,
                    this.Category,
                    this.Subcategory,
                    this.Name,
                    this.Url,
                    this.Description);
            }
        }
    }
}
