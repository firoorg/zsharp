namespace Zsharp.Bitcoin
{
    using NBitcoin;
    using Zsharp.Zcoin;

    public static class InputExtensions
    {
        public static bool IsSigmaSpend(this TxIn input) =>
            input.PrevOut.Hash == uint256.Zero &&
            input.PrevOut.N >= 1 &&
            ScriptStartsWith(input.ScriptSig, OperationCode.SigmaSpend);

        public static bool IsZerocoinRemint(this TxIn input) =>
            input.PrevOut.IsNull && ScriptStartsWith(input.ScriptSig, OperationCode.ZerocoinRemint);

        public static bool IsZerocoinSpend(this TxIn input) =>
            input.PrevOut.IsNull && ScriptStartsWith(input.ScriptSig, OperationCode.ZerocoinSpend);

        static bool ScriptStartsWith(Script script, OperationCode code) =>
            script.Length > 0 &&
            script.ToBytes(true)[0] == (byte)code;
    }
}
