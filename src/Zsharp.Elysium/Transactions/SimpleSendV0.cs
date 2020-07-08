namespace Zsharp.Elysium.Transactions
{
    using System;
    using NBitcoin;

    public class SimpleSendV0 : Elysium.Transaction
    {
        public const int StaticId = 0;

        public SimpleSendV0(BitcoinAddress sender, BitcoinAddress? receiver, PropertyId property, TokenAmount amount)
            : base(sender, receiver)
        {
            if (amount <= TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "The value is less than one.");
            }

            this.Property = property;
            this.Amount = amount;
        }

        public TokenAmount Amount { get; }

        public override int Id => StaticId;

        public PropertyId Property { get; }

        public new BitcoinAddress Sender => base.Sender!;

        public override int Version => 0;
    }
}
