using System;

namespace ErpCrmService.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all domain-related exceptions
    /// Follows Exception hierarchy design pattern
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }

        protected DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a business rule is violated
    /// </summary>
    public class BusinessRuleViolationException : DomainException
    {
        public string RuleName { get; }

        public BusinessRuleViolationException(string ruleName, string message) : base(message)
        {
            RuleName = ruleName;
        }

        public BusinessRuleViolationException(string ruleName, string message, Exception innerException) : base(message, innerException)
        {
            RuleName = ruleName;
        }
    }

    /// <summary>
    /// Exception thrown when an entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public string EntityType { get; }
        public string EntityId { get; }

        public EntityNotFoundException(string entityType, string entityId) 
            : base($"{entityType} with ID '{entityId}' was not found.")
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        public EntityNotFoundException(string entityType, string entityId, Exception innerException)
            : base($"{entityType} with ID '{entityId}' was not found.", innerException)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    /// <summary>
    /// Exception thrown when trying to create an entity that already exists
    /// </summary>
    public class EntityAlreadyExistsException : DomainException
    {
        public string EntityType { get; }
        public string ConflictingProperty { get; }
        public string ConflictingValue { get; }

        public EntityAlreadyExistsException(string entityType, string conflictingProperty, string conflictingValue)
            : base($"{entityType} with {conflictingProperty} '{conflictingValue}' already exists.")
        {
            EntityType = entityType;
            ConflictingProperty = conflictingProperty;
            ConflictingValue = conflictingValue;
        }
    }

    /// <summary>
    /// Exception thrown when insufficient stock is available for an operation
    /// </summary>
    public class InsufficientStockException : DomainException
    {
        public string ProductSku { get; }
        public int RequestedQuantity { get; }
        public int AvailableQuantity { get; }

        public InsufficientStockException(string productSku, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for product '{productSku}'. Requested: {requestedQuantity}, Available: {availableQuantity}")
        {
            ProductSku = productSku;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }

    /// <summary>
    /// Exception thrown when customer credit limit is exceeded
    /// </summary>
    public class CreditLimitExceededException : DomainException
    {
        public decimal RequestedAmount { get; }
        public decimal AvailableCredit { get; }
        public Guid CustomerId { get; }

        public CreditLimitExceededException(Guid customerId, decimal requestedAmount, decimal availableCredit)
            : base($"Credit limit exceeded for customer '{customerId}'. Requested: ${requestedAmount:F2}, Available: ${availableCredit:F2}")
        {
            CustomerId = customerId;
            RequestedAmount = requestedAmount;
            AvailableCredit = availableCredit;
        }
    }
}