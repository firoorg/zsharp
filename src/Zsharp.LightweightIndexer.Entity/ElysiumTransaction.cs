namespace Zsharp.LightweightIndexer.Entity
{
    using NBitcoin;

    public sealed class ElysiumTransaction
    {
        public ElysiumTransaction(uint256 transactionHash, string? sender, string? receiver, byte[] serialized)
        {
            this.TransactionHash = transactionHash;
            this.Sender = sender;
            this.Receiver = receiver;
            this.Serialized = serialized;
        }

        public string? Receiver { get; }

        public string? Sender { get; }

        public byte[] Serialized { get; }

        public uint256 TransactionHash { get; }
    }
}
