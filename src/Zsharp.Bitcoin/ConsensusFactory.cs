namespace Zsharp.Bitcoin
{
    using System;
    using NBitcoin;

    sealed class ConsensusFactory : NBitcoin.ConsensusFactory
    {
        readonly DateTimeOffset mtpActivated;

        public ConsensusFactory(DateTimeOffset mtpActivated)
        {
            this.mtpActivated = mtpActivated;
        }

        public override NBitcoin.Block CreateBlock() => new Block(this, (BlockHeader)this.CreateBlockHeader());

        public override NBitcoin.BlockHeader CreateBlockHeader() => new BlockHeader(this.mtpActivated);

        public override NBitcoin.Transaction CreateTransaction() => new Transaction(this);

        public override TxOut CreateTxOut() => new Output(this);
    }
}
