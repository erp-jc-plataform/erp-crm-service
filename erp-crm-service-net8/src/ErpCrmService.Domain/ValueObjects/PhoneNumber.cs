using System.Text.RegularExpressions;

namespace ErpCrmService.Domain.ValueObjects;

public class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex = new(@"^[\+]?[1-9][\d]{6,14}$", RegexOptions.Compiled);

    public string Value { get; private set; } = default!;

    private PhoneNumber() { }

    public PhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("Phone number cannot be empty");
        var cleaned = Regex.Replace(phoneNumber, @"[\s\-\(\)\.]", "");
        if (!PhoneRegex.IsMatch(cleaned)) throw new ArgumentException($"Invalid phone number format: {phoneNumber}");
        Value = cleaned;
    }

    public static implicit operator string(PhoneNumber phone) => phone?.Value ?? string.Empty;
    public static explicit operator PhoneNumber(string phone) => new(phone);

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string ToString() => Value;
}
