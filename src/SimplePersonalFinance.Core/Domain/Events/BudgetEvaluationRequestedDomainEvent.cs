using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.Events;

public class BudgetEvaluationRequestedDomainEvent:IDomainEvent
{
    public Guid AccountId { get; }
    public Guid UserId { get; set; }
    public CategoryEnum Category { get; set; }

    public DateTime OccuredOn { get; }

    public string EntityType { get; }

    public Guid EntityId { get; }

    public BudgetEvaluationRequestedDomainEvent(Guid accountId,Guid userId,CategoryEnum category)
    {
        AccountId = accountId;
        Category = category;
        UserId = userId;
        OccuredOn = DateTime.UtcNow;
        EntityType=nameof(BudgetEvaluationRequestedDomainEvent);
        EntityId = accountId;
    }
}
