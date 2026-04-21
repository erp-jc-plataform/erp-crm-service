using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Services;

public interface ICustomerDomainService
{
    Task<bool> CanCreateCustomerAsync(string email, string? taxId);
    Task<bool> ValidateBusinessRulesForCustomerCreationAsync(Customer customer);
}

public enum CustomerCreditStatus { Excellent = 1, Good = 2, Fair = 3, Poor = 4, Critical = 5 }
public enum CustomerRiskLevel { Low = 1, Medium = 2, High = 3, Critical = 4 }
