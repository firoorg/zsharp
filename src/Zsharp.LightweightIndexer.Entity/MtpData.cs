namespace Zsharp.LightweightIndexer.Entity
{
    using NBitcoin;

    public sealed class MtpData
    {
        public MtpData(uint256 blockHash, uint256 hash, int version, uint256 reserved1, uint256 reserved2)
        {
            this.BlockHash = blockHash;
            this.Hash = hash;
            this.Version = version;
            this.Reserved1 = reserved1;
            this.Reserved2 = reserved2;
        }

        public uint256 BlockHash { get; }

        public uint256 Hash { get; }

        public uint256 Reserved1 { get; }

        public uint256 Reserved2 { get; }

        public int Version { get; }
    }
}
