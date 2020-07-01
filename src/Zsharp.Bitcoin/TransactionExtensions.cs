namespace Zsharp.Bitcoin
{
    public static class TransactionExtensions
    {
        public static Elysium.Transaction? GetElysiumTransaction(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).ElysiumTransaction;

        public static bool IsSigmaSpend(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsSigmaSpend;

        public static bool IsZerocoinRemint(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsZerocoinRemint;

        public static bool IsZerocoinSpend(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsZerocoinSpend;

        public static void SetElysiumTransaction(this NBitcoin.Transaction transaction, Elysium.Transaction? value) =>
            ((Transaction)transaction).ElysiumTransaction = value;
    }
}
