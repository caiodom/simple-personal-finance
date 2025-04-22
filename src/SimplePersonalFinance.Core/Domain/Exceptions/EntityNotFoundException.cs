namespace SimplePersonalFinance.Core.Domain.Exceptions;

public class EntityNotFoundException:DomainException
{
    public string EntityName { get;}
    public Guid EntityId { get;}

    public EntityNotFoundException(string entityName, Guid entityId) : base($"Entity '{entityName}' with id '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}
