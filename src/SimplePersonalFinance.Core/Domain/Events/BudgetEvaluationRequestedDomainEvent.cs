using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.Events;

public class BudgetEvaluationRequestedDomainEvent:IDomainEvent
{
    public Guid AccountId { get; }
    public Guid UserId { get; set; }
    public CategoryEnum Category { get; set; }

    public DateTime OccuredOn { get; }

    public BudgetEvaluationRequestedDomainEvent(Guid accountId,Guid userId,CategoryEnum category)
    {
        AccountId = accountId;
        Category = category;
        UserId = userId;
        OccuredOn = DateTime.UtcNow;
    }
}
