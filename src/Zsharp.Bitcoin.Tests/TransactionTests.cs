namespace Zsharp.Bitcoin.Tests
{
    using System.IO;
    using Moq;
    using NBitcoin;
    using NBitcoin.DataEncoders;
    using Xunit;
    using Zsharp.Testing;
    using Zsharp.Zcoin;

    public sealed class TransactionTests
    {
        readonly ConsensusFactory factory;
        readonly Transaction subject;

        public TransactionTests()
        {
            this.factory = Networks.Default.Mainnet.Consensus.ConsensusFactory;
            this.subject = this.factory.CreateTransaction();
        }

        [Fact]
        public void Constructor_WhenSucceeded_ShouldInitializeProperties()
        {
            Assert.Null(this.subject.GetElysiumTransaction());
            Assert.Empty(this.subject.GetExtraPayload());
            Assert.Equal(TransactionType.Normal, this.subject.GetTransactionType());
        }

        [Fact]
        public void ElysiumTransaction_WhenAssigned_ShouldUpdated()
        {
            var value = new Mock<Elysium.Transaction>(null, null);

            this.subject.SetElysiumTransaction(value.Object);

            Assert.Same(value.Object, this.subject.GetElysiumTransaction());
        }

        [Fact]
        public void ExtraPayload_WhenAssigned_ShouldUpdated()
        {
            var value = new byte[] { 0x00, 0x01 };

            this.subject.SetExtraPayload(value);

            Assert.Equal(value, this.subject.GetExtraPayload());
        }

        [Fact]
        public void IsCoinBase_WithCoinBase_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetCoinBase1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithNormal_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetNormal1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithZecoinSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithSigmaSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsCoinBase);
        }

        [Fact]
        public void IsCoinBase_WithZerocoinRemint_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsCoinBase);
        }

        [Fact]
        public void IsSigmaSpend_WithSigmaSpend_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetSigmaSpend1.IsSigmaSpend());
        }

        [Fact]
        public void IsSigmaSpend_WithNonSigmaSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsSigmaSpend());
            Assert.False(TestTransaction.MainnetNormal1.IsSigmaSpend());
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsSigmaSpend());
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsSigmaSpend());
        }

        [Fact]
        public void IsZerocoinRemint_WithZerocoinRemint_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.RegtestZerocoinRemint1.IsZerocoinRemint());
        }

        [Fact]
        public void IsZerocoinRemint_WithNonZerocoinRemint_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetNormal1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetZerocoinSpend1.IsZerocoinRemint());
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsZerocoinRemint());
        }

        [Fact]
        public void IsZerocoinSpend_WithZerocoinSpend_ShouldReturnTrue()
        {
            Assert.True(TestTransaction.MainnetZerocoinSpend1.IsZerocoinSpend());
        }

        [Fact]
        public void IsZerocoinSpend_WithNonZerocoinSpend_ShouldReturnFalse()
        {
            Assert.False(TestTransaction.MainnetCoinBase1.IsZerocoinSpend());
            Assert.False(TestTransaction.MainnetNormal1.IsZerocoinSpend());
            Assert.False(TestTransaction.MainnetSigmaSpend1.IsZerocoinSpend());
            Assert.False(TestTransaction.RegtestZerocoinRemint1.IsZerocoinSpend());
        }

        [Fact]
        public void TransactionType_WhenAssigned_ShouldUpdated()
        {
            var value = TransactionType.Coinbase;

            this.subject.SetTransactionType(value);

            Assert.Equal(value, this.subject.GetTransactionType());
        }

        [Fact]
        public void GetConsensusFactory_WhenInvoke_ShouldReturnZcoinVersion()
        {
            var result = this.subject.GetConsensusFactory();

            Assert.Same(this.factory, result);
        }

        [Fact]
        public void ReadWrite_DeserializeOldCoinbase_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2003a0860104f79f6e5b0877ffe6dd840000000d2f6e6f64655374726174756d2f0000000007009ce4a6000000001976a914dfa17720fa101e262a8fc1a378f25a275a26952288ac00e1f505000000001976a9147d9ed014fc4e603fca7c2e3f9097fb7d0fb487fc88ac00e1f505000000001976a914bc7e5a5234db3ab82d74c396ad2b2af419b7517488ac00e1f505000000001976a914ff71b0c9c2a90c6164a50a2fb523eb54a8a6b55088ac00a3e111000000001976a9140654dd9b856f2ece1d56cb4ee5043cd9398d962c88ac00e1f505000000001976a9140b4bfb256ef4bfa360e3b9e66e53a0bd84d196bc88ac002f6859000000001976a914dec7ddb718550686e8ea9b100354ef04f20167a988ac00000000");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(1U, this.subject.Version);
            Assert.Equal(TransactionType.Normal, this.subject.GetTransactionType());
            Assert.Empty(this.subject.GetExtraPayload());
            Assert.True(this.subject.IsCoinBase);
            Assert.Single(this.subject.Inputs);
            Assert.Equal(7, this.subject.Outputs.Count);
            Assert.Equal(uint256.Parse("019e31415109ca16f09e35cab38ac0a0624949c0539ce110fbbe4cf506e9b776"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        [Fact]
        public void ReadWrite_DeserializeOldNormal_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("01000000012dd09102d8edec210d0574e89f8367055c40afd90ba061ea3733480b684a02b4010000006b483045022100d965ebacd14143307c28fcc2b4b630f138bbc50256f689f5554e805497688d0102202ccd9d87e098ef3d0dfe508c6c41b00368c2eb562b877af805efe566b9d691bd012102611eef2d8b790712b1de7b29fdc51e7eed24bf6f8f052a2c747e53d3b2958b9a0000000002a1274b00000000001976a91468bc0bc35993f640c308b176fd9760b8c798d94588aca6dc0200000000001976a9148847b2b26aa95075219d8982d8731d3022e0b3d588ac00000000");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(1U, this.subject.Version);
            Assert.Equal(TransactionType.Normal, this.subject.GetTransactionType());
            Assert.Empty(this.subject.GetExtraPayload());
            Assert.False(this.subject.IsCoinBase);
            Assert.Single(this.subject.Inputs);
            Assert.Equal(2, this.subject.Outputs.Count);
            Assert.Equal(uint256.Parse("a9b8ebadbaf5f77b1ed1421e142c24e694dfe6cee54ca46c2af4c8db15accce1"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        [Fact]
        public void ReadWrite_DeserializeEvoCoinbase_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("03000500010000000000000000000000000000000000000000000000000000000000000000ffffffff3603905b0404f110125f0800000000009d7f2e5f1f0000000000001b324d696e6572732068747470733a2f2f326d696e6572732e636f6dffffffff07004e7253000000001976a91455154ec4385f71c4a284731cafaf5d19406c030588ac8017b42c000000001976a9140bdd6ca0dc3f2933f76e2f57aff3f822cf69abd388ac80f0fa02000000001976a9147d9ed014fc4e603fca7c2e3f9097fb7d0fb487fc88ac80f0fa02000000001976a914bc7e5a5234db3ab82d74c396ad2b2af419b7517488ac80f0fa02000000001976a914ff71b0c9c2a90c6164a50a2fb523eb54a8a6b55088ac80d1f008000000001976a9140654dd9b856f2ece1d56cb4ee5043cd9398d962c88ac80f0fa02000000001976a9140b4bfb256ef4bfa360e3b9e66e53a0bd84d196bc88ac00000000260100905b040020d084ede4b0947ccb7e250435ed034fad719a963917a9d5ce5980117ca9e3f8");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(3U, this.subject.Version);
            Assert.Equal(TransactionType.Coinbase, this.subject.GetTransactionType());
            Assert.NotEmpty(this.subject.GetExtraPayload());
            Assert.True(this.subject.IsCoinBase);
            Assert.Single(this.subject.Inputs);
            Assert.Equal(7, this.subject.Outputs.Count);
            Assert.Equal(uint256.Parse("63ba8c7958cc4b4ea0f4aff68747c05119294d2312b3e06359e2aff3fd4e8eae"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        [Fact]
        public void ReadWrite_DeserializeEvoNormal_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("01000000014ee06760a8bea4869b519ab7866909b191ae5f957d75e59258ec3d8c3885602e010000006b483045022100e2ff20979ef8cfc9576e70d9280e69dd06ff6a6d2c5f376e1e5013a25c3c68de022020a598a92cd6898bfda20022e8bd381d38adb5f3978bc62f28d0e6917fd8f21a0121032e50b48c0ec886bb1c53758d69442a6325db990734fae02b80111d7046a1c674ffffffff028017ef84080000001976a914711b3eec37a64f5c811ce9132236ac31ac542bcf88ac20aa66f34e0000001976a9142983b53bf9e2fde71e5135e49117929865cae1ef88ac00000000");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(1U, this.subject.Version);
            Assert.Equal(TransactionType.Normal, this.subject.GetTransactionType());
            Assert.Empty(this.subject.GetExtraPayload());
            Assert.False(this.subject.IsCoinBase);
            Assert.Single(this.subject.Inputs);
            Assert.Equal(2, this.subject.Outputs.Count);
            Assert.Equal(uint256.Parse("ca65b37e9e61c290eae90d0bbdae79c3d0f5b4ea8a3e47a0bd6e6ac1c178932a"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        [Fact]
        public void ReadWrite_DeserializeProviderOperation_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("03000100011caef4526c1fc3791bb078b97916bd6f321600f3090350f1c509863bdba34499000000006b483045022100d3d8d587e0711a323ca0c678fa0d91108e84776ac8d8c4f81db29f1e39f437a30220209e5058eb04c00067dfe2abf7958492fee11470dd04d9d685ba890978c69086012103b453e139dd1b9bec292f0756946a238ee6d1e61c5949e72321d42416ac8b490dfeffffff018a2f0f00000000001976a91412bea64d01c2c448d601f277cbdc1d477c87b77388ac00000000fd120101000000000035beb3fbf2009717f6b20155267119422c0efb78a6d3dd7f9855d0d5d3e9623e0000000000000000000000000000ffffa516131d1fe8d0e52682d909a4624bed9e35e4671ff86f9a86e819279907a4a9d53470cca51cd5f8ea16f6a654e84a3fa40468d5c3ee390599135fa774f277c8791108a1838d8828fe6bd0e52682d909a4624bed9e35e4671ff86f9a86e800001976a914e58511a179afe5ae29fb6cfd28ed83a9d21e2d0188ac5640f90bb6135d55838678f85ff09cabc0dcce978088e47850da2eb06f47ceb3411fce1b82b28f87d07a027c7805c282472142ffa8a3ce010604313c2b111447d5494eca809fcf46f6a41afb4332627bef2c2a2a07c833bc5594aec5312d2d34b110");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(3U, this.subject.Version);
            Assert.Equal(TransactionType.RegisterProvider, this.subject.GetTransactionType());
            Assert.NotEmpty(this.subject.GetExtraPayload());
            Assert.False(this.subject.IsCoinBase);
            Assert.Single(this.subject.Inputs);
            Assert.Single(this.subject.Outputs);
            Assert.Equal(uint256.Parse("a3080e7a4e4f969d5e5ef5363b3062c76616c41be4e8bc2c5d759b7cc542e9fb"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        [Fact]
        public void ReadWrite_DeserializeQuorumCommitment_ShouldProduceSameDataWhenSerialize()
        {
            // Arrange.
            var data = Encoders.Hex.DecodeData("03000600000000000000fd490101008e5b0400010001f8ca9390162ff2fa369b16d6cadb8bbc518fb2c765e9c7f5d9d6aaee07dde4ec32ffdfefffffdf0132ffdfefffffdf0199ae31206b1a8a707f0fc558be28fe176344f6650b9f3eaf09424a826ae877969878657220842c6585cdb1997b959c3f3dc3096fcac557df4cf64435797235c9251ea76bcd1603e469b56e2a865d7f5d974dd3970c76eb474289053023b2b8d1cbd950d5c05db44fd3b47576df0a265397625387bb867f46ba78065815f4796e01bccf77feddbabc136ed4d37620ec2656ed1c93219956d8bdb658a68f904fafea5fd00c75a5653c68b8644ac6358b9188c7369aef7671c7667a9785a95d3bf46242d8eafe657d5d54138b50f3621ec8210fa02e999d6a83ae6ae44c970f331011ea5bcfd005353faea271b69555af482ffaf7ffd0b13afc28dacc077dbb0dd9abe21de8ee8dc562c10b88d73b077eb7");
            var stream = new BitcoinStream(data);

            // Act.
            this.subject.ReadWrite(stream);

            // Assert.
            Assert.Equal(3U, this.subject.Version);
            Assert.Equal(TransactionType.QuorumCommitment, this.subject.GetTransactionType());
            Assert.NotEmpty(this.subject.GetExtraPayload());
            Assert.False(this.subject.IsCoinBase);
            Assert.Empty(this.subject.Inputs);
            Assert.Empty(this.subject.Outputs);
            Assert.Equal(uint256.Parse("e081859920cc66a5324c8a08819a6fc489ceb798ee5d92f5ad9eced8c7f67f39"), this.subject.GetHash());
            Assert.Equal(data, Serialize(this.subject));
        }

        static byte[] Serialize(Transaction transaction)
        {
            using (var buffer = new MemoryStream())
            {
                var stream = new BitcoinStream(buffer, true);
                transaction.ReadWrite(stream);
                return buffer.ToArray();
            }
        }
    }
}
