namespace Zsharp.Zcoin
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using NBitcoin;

    public sealed class MtpData : IBitcoinSerializable
    {
        const int L = 64;

        readonly List<byte[]>[] nProofMTP;
        byte[] hashRootMTP;
        byte[] nBlockMTP;

        public MtpData()
        {
            this.nProofMTP = new List<byte[]>[L * 3];
            this.hashRootMTP = new byte[16];
            this.nBlockMTP = new byte[L * 2 * 128 * 8];
        }

        public void ReadWrite(BitcoinStream stream)
        {
            if (stream.Serializing)
            {
                // Write.
                stream.Inner.Write(this.hashRootMTP, 0, this.hashRootMTP.Length);
                stream.Counter.AddWritten(this.hashRootMTP.Length);

                stream.Inner.Write(this.nBlockMTP, 0, this.nBlockMTP.Length);
                stream.Counter.AddWritten(this.nBlockMTP.Length);

                for (var i = 0; i < L * 3; i++)
                {
                    // TODO: Throw if nProofMTP[i].Count is greater than 255.
                    stream.ReadWrite((byte)this.nProofMTP[i].Count);

                    foreach (var mtpData in this.nProofMTP[i])
                    {
                        // TODO: Throw if mtpData.Length is not 16.
                        stream.Inner.Write(mtpData, 0, mtpData.Length);
                        stream.Counter.AddWritten(mtpData.Length);
                    }
                }
            }
            else
            {
                // Read.
                this.hashRootMTP = stream.Inner.ReadBytes(this.hashRootMTP.Length);
                stream.Counter.AddReaden(this.hashRootMTP.Length);

                this.nBlockMTP = stream.Inner.ReadBytes(this.nBlockMTP.Length);
                stream.Counter.AddReaden(this.nBlockMTP.Length);

                for (var i = 0; i < L * 3; i++)
                {
                    byte numberOfProofBlocks = 0;

                    stream.ReadWrite(ref numberOfProofBlocks);

                    this.nProofMTP[i] = new List<byte[]>(numberOfProofBlocks);

                    for (byte j = 0; j < numberOfProofBlocks; j++)
                    {
                        var mtpData = stream.Inner.ReadBytes(16);
                        stream.Counter.AddReaden(16);

                        this.nProofMTP[i].Add(mtpData);
                    }
                }
            }
        }
    }
}
