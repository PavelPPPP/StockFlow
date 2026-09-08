using System.Text.RegularExpressions;

namespace StockFlow.Domain.ValueObjects
{
    public sealed partial record SkuValue
    {
        public string Value { get; }

        private SkuValue(string value) => Value = value;

        public static SkuValue Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("SKU cannot be null or empty.", nameof(value));
            }

            if (value.Length < 3 || value.Length > 30)
            {
                throw new ArgumentException("SKU length must be between 3 and 30 characters.", nameof(value));
            }

            if (!SkuFormatRegex().IsMatch(value))
            {
                throw new ArgumentException("SKU must contain only uppercase letters, digits, and hyphens.", nameof(value));
            }

            return new SkuValue(value);
        }

        [GeneratedRegex("^[A-Z0-9-]+$")]
        private static partial Regex SkuFormatRegex();
    }
}