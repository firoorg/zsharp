namespace Zsharp.Zcoin
{
    public enum TransactionType : short
    {
        Normal = 0,
        RegisterProvider = 1,
        UpdateProviderService = 2,
        UpdateProviderRegistration = 3,
        RevokeProviderService = 4,
        Coinbase = 5,
        QuorumCommitment = 6,
    }
}
