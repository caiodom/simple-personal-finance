using FluentValidation;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;

namespace SimplePersonalFinance.Application.Validators;

public class EditBudgetCommandValidator:AbstractValidator<EditBudgetCommand>
{
    public EditBudgetCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Budget ID is required")
            .Must(x => x != Guid.Empty)
            .WithMessage("Budget ID cannot be empty GUID");

        RuleFor(x => x.LimitAmount)
            .NotEmpty()
            .WithMessage("Limit amount is required")
            .GreaterThan(0)
            .WithMessage("Limit amount must be greater than 0");

        RuleFor(x => x.Month)
            .NotEmpty()
            .WithMessage("Month is required")
            .InclusiveBetween(1, 12)
            .WithMessage("Month must be between 1 and 12");

        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Year is required")
            .GreaterThan(0)
            .WithMessage("Year must be greater than 0");

    }
}
