namespace Zsharp.Elysium
{
    using System;

    public class TransactionSerializationException : Exception
    {
        public TransactionSerializationException()
        {
        }

        public TransactionSerializationException(string message)
            : base(message)
        {
        }

        public TransactionSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
