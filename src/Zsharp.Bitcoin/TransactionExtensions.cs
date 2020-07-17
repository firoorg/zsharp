namespace Zsharp.Bitcoin
{
    using Zsharp.Zcoin;

    public static class TransactionExtensions
    {
        public static Elysium.Transaction? GetElysiumTransaction(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).ElysiumTransaction;

        public static byte[] GetExtraPayload(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).ExtraPayload;

        public static TransactionType GetTransactionType(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).TransactionType;

        public static bool IsSigmaSpend(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsSigmaSpend;

        public static bool IsZerocoinRemint(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsZerocoinRemint;

        public static bool IsZerocoinSpend(this NBitcoin.Transaction transaction) =>
            ((Transaction)transaction).IsZerocoinSpend;

        public static void SetElysiumTransaction(this NBitcoin.Transaction transaction, Elysium.Transaction? value) =>
            ((Transaction)transaction).ElysiumTransaction = value;

        public static void SetExtraPayload(this NBitcoin.Transaction transaction, byte[] value) =>
            ((Transaction)transaction).ExtraPayload = value;

        public static void SetTransactionType(this NBitcoin.Transaction transaction, TransactionType value) =>
            ((Transaction)transaction).TransactionType = value;
    }
}
