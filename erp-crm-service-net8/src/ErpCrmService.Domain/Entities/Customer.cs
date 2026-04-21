using ErpCrmService.Domain.ValueObjects;

namespace ErpCrmService.Domain.Entities;

public class Customer : BaseEntity
{
    public string CompanyName { get; private set; } = default!;
    public string ContactFirstName { get; private set; } = default!;
    public string ContactLastName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PhoneNumber? Phone { get; private set; }
    public Address? Address { get; private set; }
    public string? TaxId { get; private set; }
    public CustomerStatus Status { get; private set; }
    public CustomerType Type { get; private set; }
    public string? Notes { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal CurrentBalance { get; private set; }

    private Customer() : base() { }

    public Customer(string companyName, string contactFirstName, string contactLastName,
        Email email, PhoneNumber? phone, Address? address, string? taxId,
        CustomerType type, decimal creditLimit = 0) : base()
    {
        if (string.IsNullOrWhiteSpace(companyName)) throw new ArgumentException("Company name cannot be empty");
        if (string.IsNullOrWhiteSpace(contactFirstName)) throw new ArgumentException("Contact first name cannot be empty");
        if (string.IsNullOrWhiteSpace(contactLastName)) throw new ArgumentException("Contact last name cannot be empty");
        CompanyName = companyName;
        ContactFirstName = contactFirstName;
        ContactLastName = contactLastName;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        Address = address;
        TaxId = taxId;
        Type = type;
        Status = CustomerStatus.Active;
        CreditLimit = creditLimit;
        CurrentBalance = 0;
    }

    public void UpdateContactInfo(string firstName, string lastName, Email email, PhoneNumber? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty");
        ContactFirstName = firstName;
        ContactLastName = lastName;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        MarkAsUpdated("System");
    }

    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        MarkAsUpdated("System");
    }

    public void UpdateCreditLimit(decimal creditLimit)
    {
        if (creditLimit < 0) throw new ArgumentException("Credit limit cannot be negative");
        CreditLimit = creditLimit;
        MarkAsUpdated("System");
    }

    public void AddBalance(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        CurrentBalance += amount;
        MarkAsUpdated("System");
    }

    public void DeductBalance(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (CurrentBalance + CreditLimit < amount) throw new InvalidOperationException("Insufficient balance and credit limit");
        CurrentBalance -= amount;
        MarkAsUpdated("System");
    }

    public void SuspendCustomer(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason for suspension is required");
        Status = CustomerStatus.Suspended;
        Notes = $"Suspended: {reason}";
        MarkAsUpdated("System");
    }

    public void ReactivateCustomer()
    {
        Status = CustomerStatus.Active;
        MarkAsUpdated("System");
    }
}

public enum CustomerStatus { Active = 1, Inactive = 2, Suspended = 3, Blocked = 4 }
public enum CustomerType { Individual = 1, Corporate = 2, Government = 3, NonProfit = 4 }
