namespace Zsharp.LightweightIndexer.Entity
{
    using System;
    using NBitcoin;

    public sealed class Output : IComparable<Output?>
    {
        public Output(uint256 transactionHash, int index, Script script, long value)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            this.TransactionHash = transactionHash;
            this.Index = index;
            this.Script = script;
            this.Value = value;
        }

        public int Index { get; }

        public uint256 TransactionHash { get; }

        public Script Script { get; }

        public long Value { get; }

        public int CompareTo(Output? other)
        {
            if (other == null)
            {
                return 1;
            }

            return this.Index - other.Index;
        }

        public override bool Equals(object? other)
        {
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.CompareTo((Output)other) == 0;
        }

        public override int GetHashCode()
        {
            return this.Index.GetHashCode();
        }
    }
}
