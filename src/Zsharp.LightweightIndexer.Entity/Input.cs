namespace Zsharp.LightweightIndexer.Entity
{
    using System;
    using NBitcoin;

    public sealed class Input : IComparable<Input?>
    {
        public Input(
            uint256 transactionHash,
            int index,
            uint256 outputHash,
            int outputIndex,
            Script script,
            int sequence)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (outputIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputIndex));
            }

            this.TransactionHash = transactionHash;
            this.Index = index;
            this.OutputHash = outputHash;
            this.OutputIndex = outputIndex;
            this.Script = script;
            this.Sequence = sequence;
        }

        public int Index { get; }

        public uint256 OutputHash { get; }

        public int OutputIndex { get; }

        public Script Script { get; }

        public int Sequence { get; }

        public uint256 TransactionHash { get; }

        public int CompareTo(Input? other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.Index - other.Index;
        }

        public override bool Equals(object other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.CompareTo((Input)other) == 0;
        }

        public override int GetHashCode()
        {
            return this.Index.GetHashCode();
        }
    }
}
