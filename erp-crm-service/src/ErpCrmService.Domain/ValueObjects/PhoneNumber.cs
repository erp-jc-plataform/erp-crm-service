using System;
using System.Text.RegularExpressions;

namespace ErpCrmService.Domain.ValueObjects
{
    /// <summary>
    /// Phone number value object with validation
    /// Supports international phone number formats
    /// </summary>
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex PhoneRegex = new Regex(
            @"^[\+]?[1-9][\d]{0,15}$",
            RegexOptions.Compiled);

        public string Value { get; private set; }
        public string CountryCode { get; private set; }
        public string Number { get; private set; }

        private PhoneNumber() { } // For serialization

        public PhoneNumber(string phoneNumber, string countryCode = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty");

            var cleanedNumber = CleanPhoneNumber(phoneNumber);
            
            if (!IsValidPhoneNumber(cleanedNumber))
                throw new ArgumentException($"Invalid phone number format: {phoneNumber}");

            Value = cleanedNumber;
            CountryCode = ExtractCountryCode(cleanedNumber, countryCode);
            Number = ExtractNumber(cleanedNumber);
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            // Remove spaces, dashes, parentheses, and other formatting
            return Regex.Replace(phoneNumber, @"[\s\-\(\)\.]", "");
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return PhoneRegex.IsMatch(phoneNumber) && phoneNumber.Length >= 7 && phoneNumber.Length <= 15;
        }

        private static string ExtractCountryCode(string phoneNumber, string providedCountryCode)
        {
            if (!string.IsNullOrEmpty(providedCountryCode))
                return providedCountryCode;

            if (phoneNumber.StartsWith("+"))
            {
                // Extract country code (typically 1-3 digits after +)
                for (int i = 1; i <= 3 && i < phoneNumber.Length; i++)
                {
                    var possibleCode = phoneNumber.Substring(1, i);
                    if (IsValidCountryCode(possibleCode))
                        return possibleCode;
                }
            }

            return "1"; // Default to US/Canada if no country code detected
        }

        private static string ExtractNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("+"))
            {
                var countryCodeLength = ExtractCountryCode(phoneNumber, null).Length;
                return phoneNumber.Substring(countryCodeLength + 1);
            }
            return phoneNumber;
        }

        private static bool IsValidCountryCode(string code)
        {
            // Simplified validation - in real world, you'd have a comprehensive list
            int codeNumber;
            return int.TryParse(code, out codeNumber) && codeNumber >= 1 && codeNumber <= 999;
        }

        public string GetFormattedNumber()
        {
            if (CountryCode == "1" && Number.Length == 10)
            {
                // US/Canada format: (555) 123-4567
                return $"({Number.Substring(0, 3)}) {Number.Substring(3, 3)}-{Number.Substring(6)}";
            }
            else
            {
                // International format: +1 555 123 4567
                return $"+{CountryCode} {Number}";
            }
        }

        public static implicit operator string(PhoneNumber phoneNumber)
        {
            return phoneNumber?.Value;
        }

        public static explicit operator PhoneNumber(string phoneNumber)
        {
            return new PhoneNumber(phoneNumber);
        }

        public bool Equals(PhoneNumber other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PhoneNumber);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return GetFormattedNumber();
        }

        public static bool operator ==(PhoneNumber left, PhoneNumber right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PhoneNumber left, PhoneNumber right)
        {
            return !Equals(left, right);
        }
    }
}