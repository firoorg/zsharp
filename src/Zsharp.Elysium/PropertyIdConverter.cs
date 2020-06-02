namespace Zsharp.Elysium
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class PropertyIdConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) ||
            sourceType == typeof(int) ||
            sourceType == typeof(long);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string) ||
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
                        return PropertyId.Parse(s);
                    }
                    catch (FormatException ex)
                    {
                        throw new NotSupportedException($"Cannot convert {s} to {typeof(PropertyId)}.", ex);
                    }

                case long n:
                    try
                    {
                        return new PropertyId(n);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        throw new NotSupportedException($"Cannot convert {n} to {typeof(PropertyId)}.", ex);
                    }

                default:
                    throw new NotSupportedException(
                        $"Don't know how to convert {value.GetType()} to {typeof(PropertyId)}.");
            }
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            var id = (PropertyId)value;

            if (destinationType == typeof(string))
            {
                return id.ToString();
            }
            else if (destinationType == typeof(long))
            {
                return id.Value;
            }
            else
            {
                throw new NotSupportedException(
                    $"Don't know how to convert {typeof(PropertyId)} to {destinationType}.");
            }
        }
    }
}
