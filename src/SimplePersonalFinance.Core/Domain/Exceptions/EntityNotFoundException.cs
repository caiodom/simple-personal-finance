namespace SimplePersonalFinance.Core.Domain.Exceptions;

public class EntityNotFoundException:DomainException
{
    public string EntityName { get;}
    public Guid EntityId { get;}

    public EntityNotFoundException(string entityName, Guid entityId,string? message =null) : base((string.IsNullOrEmpty(message))? $"Entity '{entityName}' with id '{entityId}' was not found.":message)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
