using FluentValidation;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Validators;

public class CreateAccountCommandValidator:AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator(IUnitOfWork uow)
    {
       RuleFor(x=>x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .Must(x => x != Guid.Empty)
            .WithMessage("User ID cannot be empty GUID")
            .MustAsync(async (id,_)=> await uow.Users.GetByIdAsync(id) != null)
            .WithMessage("User not found");

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
