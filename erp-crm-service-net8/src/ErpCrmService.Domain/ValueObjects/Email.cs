using System.Text.RegularExpressions;

namespace ErpCrmService.Domain.ValueObjects;

public class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; private set; } = default!;

    private Email() { }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty");
        if (!EmailRegex.IsMatch(email)) throw new ArgumentException($"Invalid email format: {email}");
        Value = email.ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email?.Value ?? string.Empty;
    public static explicit operator Email(string email) => new(email);

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Email);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string ToString() => Value;
}
