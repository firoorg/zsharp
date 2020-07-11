namespace Zsharp.Elysium.Transactions
{
    using NBitcoin;

    public class CreateManagedPropertyV1 : CreateManagedPropertyV0
    {
        public CreateManagedPropertyV1(
            BitcoinAddress sender,
            BitcoinAddress? receiver,
            string name,
            string category,
            string subcategory,
            string website,
            string description,
            PropertyType type,
            TokenType tokenType,
            PrivateTransactionStatus privateTransactionStatus,
            PropertyId? previousId)
            : base(sender, receiver, name, category, subcategory, website, description, type, tokenType, previousId)
        {
            this.PrivateTransactionStatus = privateTransactionStatus;
        }

        public PrivateTransactionStatus PrivateTransactionStatus { get; }

        public override int Version => 1;
    }
}
