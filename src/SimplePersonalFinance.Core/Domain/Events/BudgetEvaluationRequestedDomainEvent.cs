using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.Events;

public class BudgetEvaluationRequestedDomainEvent:IDomainEvent
{
    public Guid AccountId { get; }
    public CategoryEnum Category { get; set; }

    public DateTime OccuredOn { get; }

    public BudgetEvaluationRequestedDomainEvent(Guid accountId,CategoryEnum category)
    {
        AccountId = accountId;
        Category = category;
        OccuredOn = DateTime.UtcNow;
    }
}
