namespace Zsharp.Testing
{
    using NBitcoin;
    using Zsharp.Bitcoin;

    public static class TestAddress
    {
        public static readonly BitcoinAddress Mainnet1 = BitcoinAddress.Create(
            "a8ULhhDgfdSiXJhSZVdhb8EuDc6R3ogsaM",
            Networks.Default.Mainnet);

        public static readonly BitcoinAddress Mainnet2 = BitcoinAddress.Create(
            "aGyHXMXvAgxts3o9YHvuyk9dZAZawiG9VD",
            Networks.Default.Mainnet);

        public static readonly BitcoinAddress Testnet1 = BitcoinAddress.Create(
            "TEDC38GBncNgtd2pVXeDhLeUGwJmXsiJBA",
            Networks.Default.Testnet);

        public static readonly BitcoinAddress Testnet2 = BitcoinAddress.Create(
            "TG3Pnw5xPZQS8JXMVa3F9WjUFfUqXKsqAz",
            Networks.Default.Testnet);

        public static readonly BitcoinAddress Regtest1 = BitcoinAddress.Create(
            "TEDC38GBncNgtd2pVXeDhLeUGwJmXsiJBA",
            Networks.Default.Regtest);

        public static readonly BitcoinAddress Regtest2 = BitcoinAddress.Create(
            "TG3Pnw5xPZQS8JXMVa3F9WjUFfUqXKsqAz",
            Networks.Default.Regtest);

        public static readonly BitcoinAddress Regtest3 = BitcoinAddress.Create(
            "TG2AZoHbcaPAT6WZr5ywEVg8FH9DUVqSck",
            Networks.Default.Regtest);
    }
}
