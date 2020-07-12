namespace Zsharp.Elysium.Transactions
{
    using System;
    using NBitcoin;

    public class GrantTokensV0 : Elysium.Transaction
    {
        public const int StaticId = 55;

        public GrantTokensV0(BitcoinAddress sender, BitcoinAddress? receiver, PropertyId property, TokenAmount amount)
            : base(sender, receiver)
        {
            if (amount <= TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            this.Property = property;
            this.Amount = amount;
        }

        public TokenAmount Amount { get; }

        public override int Id => StaticId;

        public PropertyId Property { get; }

        public override int Version => 0;
    }
}
