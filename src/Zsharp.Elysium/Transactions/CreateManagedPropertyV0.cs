namespace Zsharp.Elysium.Transactions
{
    using System;
    using NBitcoin;

    public class CreateManagedPropertyV0 : Elysium.Transaction
    {
        public const int StaticId = 54;

        public CreateManagedPropertyV0(
            BitcoinAddress sender,
            BitcoinAddress? receiver,
            string name,
            string category,
            string subcategory,
            string website,
            string description,
            PropertyType type,
            TokenType tokenType,
            PropertyId? previousId)
            : base(sender, receiver)
        {
            if (name.Length == 0)
            {
                throw new ArgumentException("The value is not a valid name.", nameof(name));
            }

            this.Name = name;
            this.Category = category;
            this.Subcategory = subcategory;
            this.Website = website;
            this.Description = description;
            this.Type = type;
            this.TokenType = tokenType;
            this.PreviousId = previousId;
        }

        public string Category { get; }

        public string Description { get; }

        public override int Id => StaticId;

        public string Name { get; }

        public PropertyId? PreviousId { get; }

        public string Subcategory { get; }

        public TokenType TokenType { get; }

        public PropertyType Type { get; }

        public string Website { get; }

        public override int Version => 0;
    }
}
