namespace Zsharp.Elysium.Tests
{
    using NBitcoin;

    sealed class FakeTransaction : Zsharp.Elysium.Transaction
    {
        public FakeTransaction(BitcoinAddress? sender, BitcoinAddress? receiver)
            : base(sender, receiver)
        {
            Id = 1;
            Version = 1;
        }

        public FakeTransaction(BitcoinAddress? sender, BitcoinAddress? receiver, int id, int version)
            : base(sender, receiver)
        {
            Id = id;
            Version = version;
        }

        public override int Id { get; }

        public override int Version { get; }
    }
}
