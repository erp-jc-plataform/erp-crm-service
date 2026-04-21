namespace ErpCrmService.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }
    public BusinessRuleViolationException(string ruleName, string message) : base(message) { RuleName = ruleName; }
}

public class EntityNotFoundException : DomainException
{
    public string EntityType { get; }
    public string EntityId { get; }
    public EntityNotFoundException(string entityType, string entityId)
        : base($"{entityType} with ID '{entityId}' was not found.")
    { EntityType = entityType; EntityId = entityId; }
}

public class EntityAlreadyExistsException : DomainException
{
    public string EntityType { get; }
    public string ConflictingProperty { get; }
    public string ConflictingValue { get; }
    public EntityAlreadyExistsException(string entityType, string conflictingProperty, string conflictingValue)
        : base($"{entityType} with {conflictingProperty} '{conflictingValue}' already exists.")
    { EntityType = entityType; ConflictingProperty = conflictingProperty; ConflictingValue = conflictingValue; }
}
