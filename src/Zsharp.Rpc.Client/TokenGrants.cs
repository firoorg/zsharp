namespace Zsharp.Rpc.Client
{
    using System;
    using System.Collections.Generic;
    using NBitcoin;
    using Zsharp.Elysium;

    public sealed class TokenGrants
    {
        public TokenGrants(
            PropertyId propertyId,
            string propertyName,
            BitcoinAddress propertyIssuer,
            uint256 propertyCreationTransaction,
            TokenAmount totalTokens,
            IEnumerable<TokenGrantHistory> grantHistories)
        {
            if (totalTokens < TokenAmount.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(totalTokens));
            }

            this.PropertyId = propertyId;
            this.PropertyName = propertyName;
            this.PropertyIssuer = propertyIssuer;
            this.PropertyCreationTransaction = propertyCreationTransaction;
            this.TotalTokens = totalTokens;
            this.GrantHistories = grantHistories;
        }

        public IEnumerable<TokenGrantHistory> GrantHistories { get; }

        public uint256 PropertyCreationTransaction { get; }

        public PropertyId PropertyId { get; }

        public BitcoinAddress PropertyIssuer { get; }

        public string PropertyName { get; }

        public TokenAmount TotalTokens { get; }
    }
}
