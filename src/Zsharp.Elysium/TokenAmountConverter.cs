namespace Zsharp.Elysium
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class TokenAmountConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) ||
            sourceType == typeof(int) ||
            sourceType == typeof(long) ||
            sourceType == typeof(decimal);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(long);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Normalize value.
            switch (value)
            {
                case int i:
                    value = (long)i;
                    break;
            }

            // Convert.
            switch (value)
            {
                case string s:
                    try
                    {
                        return TokenAmount.Parse(s);
                    }
                    catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                    {
                        throw new NotSupportedException($"Cannot convert {s} to {typeof(TokenAmount)}.", ex);
                    }

                case long n:
                    return new TokenAmount(n);
                case decimal d:
                    try
                    {
                        return TokenAmount.FromDivisible(d);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new NotSupportedException($"Cannot convert {d} to {typeof(TokenAmount)}.", ex);
                    }

                default:
                    throw new NotSupportedException(
                        $"Don't know how to convert {value.GetType()} to {typeof(TokenAmount)}.");
            }
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            var amount = (TokenAmount)value;

            if (destinationType == typeof(long))
            {
                return amount.Value;
            }
            else
            {
                throw new NotSupportedException($"Don't know how to convert {value.GetType()} to {destinationType}.");
            }
        }
    }
}
