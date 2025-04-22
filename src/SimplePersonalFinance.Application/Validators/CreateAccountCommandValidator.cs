using FluentValidation;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Validators;

public class CreateAccountCommandValidator:AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .WithMessage("Account type is required")
            .IsInEnum();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.InitialBalance)
            .NotEmpty()
            .WithMessage("Initial balance is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Initial balance must be greater than or equal to 0");



    }
}
