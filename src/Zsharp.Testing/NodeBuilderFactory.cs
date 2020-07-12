namespace Zsharp.Testing
{
    using System;
    using NBitcoin.Tests;
    using Zsharp.Bitcoin;

    public static class NodeBuilderFactory
    {
        static readonly NodeDownloadData DownloadData = new NodeDownloadData()
        {
            Version = "0.14.0.2",
            RegtestFolderName = "regtest",
            Linux = new NodeOSDownloadData()
            {
                Archive = "zcoin-{0}-linux64.tar.gz",
                DownloadLink = "https://github.com/zcoinofficial/zcoin/releases/download/v{0}/zcoin-{0}-linux64.tar.gz",
                Executable = "zcoin-0.14.0/bin/zcoind",
                Hash = "2b7a7d61dc4ab18883858bbcba6d56346a5aa6711f8c4b23ed5561b515f7b822",
            },
            Windows = new NodeOSDownloadData()
            {
                Archive = "zcoin-{0}-win64.zip",
                DownloadLink = "https://github.com/zcoinofficial/zcoin/releases/download/v{0}/zcoin-{0}-win64.zip",
                Executable = "zcoin-0.14.0/bin/zcoind.exe",
                Hash = "cfebe5062710bc6086187047dcb48f5e4251271e75140ec8b0d8fa989b98f475",
            },
        };

        /// <summary>
        /// Create a new instance of <see cref="NodeBuilder"/> for the specified test suite.
        /// </summary>
        /// <remarks>
        /// This method is not thread-safe.
        /// </remarks>
        public static NodeBuilder CreateNodeBuilder(Type suite)
        {
            return NodeBuilder.Create(DownloadData, Networks.Default.Regtest, suite.FullName);
        }
    }
}
