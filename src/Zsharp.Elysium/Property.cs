namespace Zsharp.Elysium
{
    public sealed class Property
    {
        public Property(
            PropertyId id,
            string name,
            string category,
            string subcategory,
            string websiteUrl,
            string description,
            TokenType tokenType)
        {
            this.Id = id;
            this.Name = name;
            this.Category = category;
            this.Subcategory = subcategory;
            this.WebsiteUrl = websiteUrl;
            this.Description = description;
            this.TokenType = tokenType;
        }

        public string Category { get; }

        public string Description { get; }

        public PropertyId Id { get; }

        public string Name { get; }

        public string Subcategory { get; }

        public TokenType TokenType { get; }

        public string WebsiteUrl { get; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return ((Property)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
