using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;

public class CreateUserCommandHandler(IAuthService authService, IUserRepository users, IUnitOfWork uow) : IRequestHandler<CreateUserCommand, ResultViewModel<Guid>>
{
    private const string DEFAULT_ROLE = "client";

    public async Task<ResultViewModel<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await users.CheckEmailAsync(request.Email);

        if (emailExists)
            throw new BusinessRuleViolationException("Duplicated Email", "Email already exists");

        var passwordHash = authService.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash, DEFAULT_ROLE, request.BirthDate).Value;

        await users.AddAsync(user);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(user.Id);
    }
}
