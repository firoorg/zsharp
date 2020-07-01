namespace Zsharp.Bitcoin
{
    using NBitcoin;
    using Zsharp.Zcoin;

    public static class BlockHeaderExtensions
    {
        public static MtpData? GetMtpData(this NBitcoin.BlockHeader header) => ((BlockHeader)header).MtpData;

        public static uint256 GetMtpHash(this NBitcoin.BlockHeader header) => ((BlockHeader)header).MtpHash;

        public static int GetMtpVersion(this NBitcoin.BlockHeader header) => ((BlockHeader)header).MtpVersion;

        public static uint256 GetReserved1(this NBitcoin.BlockHeader header) => ((BlockHeader)header).Reserved1;

        public static uint256 GetReserved2(this NBitcoin.BlockHeader header) => ((BlockHeader)header).Reserved2;

        public static bool IsMtp(this NBitcoin.BlockHeader header) => ((BlockHeader)header).IsMtp;

        public static void SetMtpData(this NBitcoin.BlockHeader header, MtpData? value) =>
            ((BlockHeader)header).MtpData = value;

        public static void SetMtpHash(this NBitcoin.BlockHeader header, uint256 value) =>
            ((BlockHeader)header).MtpHash = value;

        public static void SetMtpVersion(this NBitcoin.BlockHeader header, int value) =>
            ((BlockHeader)header).MtpVersion = value;

        public static void SetReserved1(this NBitcoin.BlockHeader header, uint256 value) =>
            ((BlockHeader)header).Reserved1 = value;

        public static void SetReserved2(this NBitcoin.BlockHeader header, uint256 value) =>
            ((BlockHeader)header).Reserved2 = value;
    }
}
