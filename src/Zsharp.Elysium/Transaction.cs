namespace Zsharp.Elysium
{
    using NBitcoin;

    public abstract class Transaction
    {
        public const int MaxId = ushort.MaxValue;
        public const int MaxVersion = ushort.MaxValue;
        public const int MinId = ushort.MinValue;
        public const int MinVersion = ushort.MinValue;

        protected Transaction(BitcoinAddress? sender, BitcoinAddress? receiver)
        {
            this.Sender = sender;
            this.Receiver = receiver;
        }

        public abstract int Id { get; }

        public BitcoinAddress? Receiver { get; }

        public BitcoinAddress? Sender { get; }

        public abstract int Version { get; }

        public static bool IsValidId(int id)
        {
            return id >= MinId && id <= MaxId;
        }

        public static bool IsValidVersion(int version)
        {
            return version >= MinVersion && version <= MaxVersion;
        }
    }
}
