namespace Zsharp.Rpc.Client
{
    using System;
    using NBitcoin;

    public sealed class ElysiumTransaction
    {
        public ElysiumTransaction(uint256 id, int type, int version, string name, Money fee, bool owned)
        {
            if (fee < Money.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(fee));
            }

            this.Id = id;
            this.Type = type;
            this.Version = version;
            this.Name = name;
            this.Fee = fee;
            this.Owned = owned;
        }

        public ElysiumConfirmation? Confirmation { get; set; }

        public Money Fee { get; }

        public uint256 Id { get; }

        public string Name { get; }

        public bool Owned { get; }

        public BitcoinAddress? ReferenceAddress { get; set; }

        public BitcoinAddress? SendingAddress { get; set; }

        public int Type { get; }

        public int Version { get; }
    }
}
