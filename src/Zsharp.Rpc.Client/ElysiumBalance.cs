namespace Zsharp.Rpc.Client
{
    using System;
    using Zsharp.Elysium;

    public sealed class ElysiumBalance
    {
        public ElysiumBalance(TokenAmount balance, TokenAmount reserved)
        {
            if (balance < TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(balance));
            }

            if (reserved < TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(reserved));
            }

            this.Balance = balance;
            this.Reserved = reserved;
        }

        public TokenAmount Balance { get; }

        public TokenAmount Reserved { get; }
    }
}
