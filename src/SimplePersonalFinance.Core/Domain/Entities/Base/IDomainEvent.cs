namespace SimplePersonalFinance.Core.Domain.Entities.Base;

public interface IDomainEvent
{
    DateTime OccuredOn { get; }
    string EntityType { get; }
    Guid EntityId { get; }
}
