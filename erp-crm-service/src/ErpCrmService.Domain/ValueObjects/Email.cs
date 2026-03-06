using System;
using System.Text.RegularExpressions;

namespace ErpCrmService.Domain.ValueObjects
{
    /// <summary>
    /// Email value object that encapsulates email validation logic
    /// Implements Value Object pattern from Domain-Driven Design
    /// </summary>
    public class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; private set; }

        private Email() { } // For serialization

        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty");

            if (!IsValidEmail(email))
                throw new ArgumentException($"Invalid email format: {email}");

            Value = email.ToLowerInvariant();
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return EmailRegex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static implicit operator string(Email email)
        {
            return email?.Value;
        }

        public static explicit operator Email(string email)
        {
            return new Email(email);
        }

        public bool Equals(Email other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Email);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(Email left, Email right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Email left, Email right)
        {
            return !Equals(left, right);
        }
    }
}