using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Application.ViewModels.Budgets;

public class BudgetViewModel
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal LimitAmount { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    public static BudgetViewModel FromEntity(Budget budget)
    {
        return new BudgetViewModel
        {
            Id = budget.Id,
            Month = budget.Month,
            Year = budget.Year,
            LimitAmount = budget.LimitAmount,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name
        };
    }
}
