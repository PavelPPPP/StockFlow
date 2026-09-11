using System.Text.RegularExpressions;

namespace StockFlow.Domain.ValueObjects
{
    public record EmailValue
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        public string Value { get; }

        private EmailValue(string value)
        {
            Value = value;
        }

        public static EmailValue Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email cannot be empty.", nameof(value));
            }

            if (!EmailRegex.IsMatch(value))
            {
                throw new ArgumentException($"'{value}' is not a valid email format.", nameof(value));
            }

            return new EmailValue(value);
        }
    }
}
