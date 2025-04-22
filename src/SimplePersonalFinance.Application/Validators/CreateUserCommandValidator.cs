using FluentValidation;
using SimplePersonalFinance.Application.Commands.CreateUser;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Application.Validators;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .Must(email =>
            {
                var result= Email.Create(email);
                return result.IsSuccess;
            })
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Birth date is required")
            .LessThan(DateTime.Now)
            .WithMessage("Birth date must be in the past");
    }
}
