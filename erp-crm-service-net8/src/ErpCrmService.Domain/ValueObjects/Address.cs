using System.Text.RegularExpressions;

namespace ErpCrmService.Domain.ValueObjects;

public class Address : IEquatable<Address>
{
    public string Street { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string Country { get; private set; } = default!;

    private Address() { }

    public Address(string street, string city, string? state, string? postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street cannot be empty");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country cannot be empty");
        Street = street.Trim(); City = city.Trim(); State = state?.Trim();
        PostalCode = postalCode?.Trim(); Country = country.Trim();
    }

    public string GetFullAddress() =>
        string.Join(", ", new[] { Street, City, State, PostalCode, Country }
            .Where(p => !string.IsNullOrWhiteSpace(p)));

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Street, other.Street, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Address);

    public override int GetHashCode() =>
        HashCode.Combine(Street?.ToUpperInvariant(), City?.ToUpperInvariant(), Country?.ToUpperInvariant());

    public override string ToString() => GetFullAddress();
}
