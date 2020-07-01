namespace Zsharp.Bitcoin
{
    using NBitcoin;

    public static class InputExtensions
    {
        public static bool IsSigmaSpend(this TxIn input) => ((Input)input).IsSigmaSpend;

        public static bool IsZerocoinRemint(this TxIn input) => ((Input)input).IsZerocoinRemint;

        public static bool IsZerocoinSpend(this TxIn input) => ((Input)input).IsZerocoinSpend;
    }
}
