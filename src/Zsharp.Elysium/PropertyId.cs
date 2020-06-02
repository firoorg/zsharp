namespace Zsharp.Elysium
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(PropertyIdConverter))]
    public sealed class PropertyId
    {
        public const long MaxValue = uint.MaxValue;
        public const long MinValue = 1;

        public PropertyId(long value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value is not valid.");
            }

            this.Value = value;
        }

        public long Value { get; }

        public static bool operator ==(PropertyId? first, PropertyId? second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            // Don't use == here due to it will causing recursive.
            if (ReferenceEquals(first, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(PropertyId? first, PropertyId? second) => !(first == second);

        public static PropertyId Parse(string s)
        {
            try
            {
                return new PropertyId(long.Parse(s));
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is OverflowException)
            {
                throw new FormatException("The value is not valid.", ex);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return ((PropertyId)obj).Value == this.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
