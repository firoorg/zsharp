namespace Zsharp.Rpc.Client
{
    using System;
    using NBitcoin;
    using Zsharp.Elysium;

    public sealed class TokenGrantHistory
    {
        public TokenGrantHistory(TokenGrantType type, uint256 transaction, TokenAmount amount)
        {
            if (amount <= TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            this.Type = type;
            this.Transaction = transaction;
            this.Amount = amount;
        }

        public TokenAmount Amount { get; }

        public uint256 Transaction { get; }

        public TokenGrantType Type { get; }
    }
}
