namespace Zsharp.Elysium
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    [TypeConverter(typeof(TokenAmountConverter))]
    public readonly struct TokenAmount : IEquatable<TokenAmount>
    {
        public TokenAmount(long value)
        {
            this.Value = value;
        }

        public long Value { get; }

        public static TokenAmount operator +(TokenAmount first, TokenAmount second) => new TokenAmount(
            checked(first.Value + second.Value));

        public static TokenAmount operator /(TokenAmount dividend, long divisor) => new TokenAmount(
            checked(dividend.Value / divisor));

        public static bool operator ==(TokenAmount first, TokenAmount second) => first.Equals(second);

        public static bool operator >(TokenAmount first, TokenAmount second) => first.Value > second.Value;

        public static bool operator >=(TokenAmount first, TokenAmount second) => first.Value >= second.Value;

        public static bool operator !=(TokenAmount first, TokenAmount second) => !first.Equals(second);

        public static bool operator <(TokenAmount first, TokenAmount second) => first.Value < second.Value;

        public static bool operator <=(TokenAmount first, TokenAmount second) => first.Value <= second.Value;

        public static TokenAmount operator -(TokenAmount amount) => Negate(amount);

        public static TokenAmount FromDivisible(decimal value)
        {
            if (value % 0.00000001m != 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value has too much precision.");
            }

            value *= 100000000m;

            if (value < long.MinValue || value > long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value is not valid.");
            }

            return new TokenAmount((long)value);
        }

        public static TokenAmount Negate(TokenAmount amount) => new TokenAmount(checked(-amount.Value));

        /// <summary>
        /// Convert a string that represents token amount.
        /// </summary>
        /// <param name="s">
        /// The string to convert. The value will be treated as divisible if it contains a period; otherwise it will be
        /// treated as indivisible.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is not in the correct format.
        /// </exception>
        /// <exception cref="OverflowException">
        /// <paramref name="s"/> is less than minimum or greater than maximum value.
        /// </exception>
        public static TokenAmount Parse(string s)
        {
            // Convert string to decimal first. We use decimal to be able to support both divisible and indivisible.
            var value = decimal.Parse(s);

            // Determine type. We need to check if there is a period in the string instead of modulo otherwise '1.0'
            // will be treated as indivisible.
            if (s.IndexOf('.') != -1)
            {
                if (value % 0.00000001m != 0m)
                {
                    throw new FormatException("Too much precision.");
                }

                value *= 100000000m;
            }

            if (value < long.MinValue || value > long.MaxValue)
            {
                throw new OverflowException("The value is not in the valid range.");
            }

            return new TokenAmount((long)value);
        }

        public bool Equals(TokenAmount other) => this.Value == other.Value;

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((TokenAmount)obj);
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public decimal ToDivisible() => this.Value / 100000000m;

        public string ToString(TokenType type)
        {
            // Don't override Object.ToString() due to we want it to return an invalid amount.
            switch (type)
            {
                case TokenType.Divisible:
                    return (this.Value / 100000000m).ToString("0.00000000", CultureInfo.InvariantCulture);
                case TokenType.Indivisible:
                    return this.Value.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "The value is not valid.");
            }
        }
    }
}
