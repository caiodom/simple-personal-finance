namespace SimplePersonalFinance.Core.Entities.Base;

public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    protected Entity()
    {
        Id= Guid.NewGuid();
        CreatedAt= DateTime.UtcNow;
        IsActive = true;
    }

    public void SetAsDeleted()
    {
        IsActive = false;
    }
}
