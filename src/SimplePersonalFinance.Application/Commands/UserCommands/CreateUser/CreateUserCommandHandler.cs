using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResultViewModel<Guid>>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _uow;
    private const string DEFAULT_ROLE = "client";
    public CreateUserCommandHandler(IAuthService authService, IUnitOfWork uow)
    {
        _authService = authService;
        _uow = uow;
    }

    public async Task<ResultViewModel<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _uow.Users.CheckEmailAsync(request.Email);

        if (emailExists)
            throw new BusinessRuleViolationException("Duplicated Email","Email already exists");

        var passwordHash = _authService.ComputeSha256Hash(request.Password);

        var user = User.Create(request.Name, request.Email, passwordHash, DEFAULT_ROLE, request.BirthDate).Value;

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(user.Id);
    }
}

