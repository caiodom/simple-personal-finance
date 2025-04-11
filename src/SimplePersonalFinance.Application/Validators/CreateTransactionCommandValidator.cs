using FluentValidation;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Application.Commands.CreateTransaction;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Validators
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateAccountTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {

            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("Account ID is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("Account ID cannot be empty GUID");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category ID is required")
                .IsInEnum();

            RuleFor(x => x.TransactionTypeId)
                .NotEmpty()
                .WithMessage("Transaction type ID is required")
                .IsInEnum();

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MinimumLength(3)
                .WithMessage("Description must be at least 3 characters long");

            RuleFor(x => x.Amount)
                .NotEmpty()
                .WithMessage("Amount is required")
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required");

        }
    }
}
