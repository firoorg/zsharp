namespace Zsharp.Bitcoin
{
    using System;
    using NBitcoin;
    using Zsharp.Zcoin;

    sealed class BlockHeader : NBitcoin.BlockHeader
    {
        static readonly DateTimeOffset GenesisBlockTime = DateTimeOffset.FromUnixTimeSeconds(1414776286);

        readonly DateTimeOffset mtpActivated;
        int mtpVersion;
        uint256 mtpHash;
        uint256 reserved1;
        uint256 reserved2;
        MtpData? mtpData;

        #pragma warning disable CS0618
        public BlockHeader(DateTimeOffset mtpActivated)
        {
            this.mtpActivated = mtpActivated;
            this.mtpVersion = 0x1000;
            this.mtpHash = uint256.Zero;
            this.reserved1 = uint256.Zero;
            this.reserved2 = uint256.Zero;
        }
        #pragma warning restore CS0618

        public bool IsMtp => this.BlockTime > GenesisBlockTime && this.BlockTime >= this.mtpActivated;

        public MtpData? MtpData
        {
            get { return this.mtpData; }
            set { this.mtpData = value; }
        }

        public uint256 MtpHash
        {
            get { return this.mtpHash; }
            set { this.mtpHash = value; }
        }

        public int MtpVersion
        {
            get { return this.mtpVersion; }
            set { this.mtpVersion = value; }
        }

        public uint256 Reserved1
        {
            get { return this.reserved1; }
            set { this.reserved1 = value; }
        }

        public uint256 Reserved2
        {
            get { return this.reserved2; }
            set { this.reserved2 = value; }
        }

        public override void ReadWrite(BitcoinStream stream)
        {
            base.ReadWrite(stream);

            if (this.IsMtp)
            {
                stream.ReadWrite(ref this.mtpVersion);
                stream.ReadWrite(ref this.mtpHash);
                stream.ReadWrite(ref this.reserved1);
                stream.ReadWrite(ref this.reserved2);

                if (stream.Serializing)
                {
                    // Write.
                    if (this.mtpData != null && stream.Type != SerializationType.Hash)
                    {
                        stream.ReadWrite(ref this.mtpData);
                    }
                }
                else
                {
                    // Read.
                    stream.ReadWrite(ref this.mtpData);
                }
            }
        }
    }
}
