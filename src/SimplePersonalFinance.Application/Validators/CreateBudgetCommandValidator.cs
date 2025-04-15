using FluentValidation;
using SimplePersonalFinance.Application.Commands.CreateBudget;

namespace SimplePersonalFinance.Application.Validators;

public class CreateBudgetCommandValidator:AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .Must(x => x != Guid.Empty)
            .WithMessage("User ID cannot be empty GUID");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category ID is required")
            .IsInEnum();

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
