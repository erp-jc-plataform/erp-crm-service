using System;
using System.Threading.Tasks;
using ErpCrmService.Domain.Entities;

namespace ErpCrmService.Domain.Services
{
    /// <summary>
    /// Domain service for customer-related business logic
    /// Implements business rules that don't naturally fit within a single entity
    /// Follows Single Responsibility and Domain-Driven Design principles
    /// </summary>
    public interface ICustomerDomainService
    {
        Task<bool> CanCreateCustomerAsync(string email, string taxId);
        Task<decimal> CalculateCustomerLifetimeValueAsync(Guid customerId);
        Task<CustomerCreditStatus> EvaluateCreditStatusAsync(Guid customerId);
        Task<bool> ValidateBusinessRulesForCustomerCreationAsync(Customer customer);
        Task<CustomerRiskLevel> AssessCustomerRiskAsync(Guid customerId);
    }

    /// <summary>
    /// Customer credit status enumeration
    /// </summary>
    public enum CustomerCreditStatus
    {
        Excellent = 1,
        Good = 2,
        Fair = 3,
        Poor = 4,
        Critical = 5
    }

    /// <summary>
    /// Customer risk level enumeration
    /// </summary>
    public enum CustomerRiskLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}