using System;

namespace ErpCrmService.Domain.Entities
{
    /// <summary>
    /// Base entity class that provides common properties for all domain entities
    /// Implements Domain-Driven Design principles
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public string CreatedBy { get; protected set; }
        public string UpdatedBy { get; protected set; }
        public bool IsActive { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        protected BaseEntity(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void MarkAsUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            BaseEntity item = (BaseEntity)obj;
            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}