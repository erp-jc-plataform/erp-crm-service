using System;

namespace ErpCrmService.Domain.ValueObjects
{
    /// <summary>
    /// Address value object representing a physical address
    /// Immutable value object following DDD principles
    /// </summary>
    public class Address : IEquatable<Address>
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string PostalCode { get; private set; }
        public string Country { get; private set; }

        private Address() { } // For serialization

        public Address(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be null or empty");

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty");

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be null or empty");

            Street = street.Trim();
            City = city.Trim();
            State = state?.Trim();
            PostalCode = postalCode?.Trim();
            Country = country.Trim();
        }

        public string GetFullAddress()
        {
            var parts = new[]
            {
                Street,
                City,
                State,
                PostalCode,
                Country
            };

            return string.Join(", ", Array.FindAll(parts, part => !string.IsNullOrWhiteSpace(part)));
        }

        public Address UpdateStreet(string newStreet)
        {
            return new Address(newStreet, City, State, PostalCode, Country);
        }

        public Address UpdateCity(string newCity)
        {
            return new Address(Street, newCity, State, PostalCode, Country);
        }

        public Address UpdateState(string newState)
        {
            return new Address(Street, City, newState, PostalCode, Country);
        }

        public Address UpdatePostalCode(string newPostalCode)
        {
            return new Address(Street, City, State, newPostalCode, Country);
        }

        public Address UpdateCountry(string newCountry)
        {
            return new Address(Street, City, State, PostalCode, newCountry);
        }

        public bool Equals(Address other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Street, other.Street, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(State, other.State, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(PostalCode, other.PostalCode, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Address);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (Street?.ToUpperInvariant().GetHashCode() ?? 0);
                hash = hash * 23 + (City?.ToUpperInvariant().GetHashCode() ?? 0);
                hash = hash * 23 + (State?.ToUpperInvariant().GetHashCode() ?? 0);
                hash = hash * 23 + (PostalCode?.ToUpperInvariant().GetHashCode() ?? 0);
                hash = hash * 23 + (Country?.ToUpperInvariant().GetHashCode() ?? 0);
                return hash;
            }
        }

        public override string ToString()
        {
            return GetFullAddress();
        }

        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }
    }
}