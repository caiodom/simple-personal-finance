using MediatR;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _uow;
    private const string DEFAULT_ROLE = "client";
    public CreateUserCommandHandler(IAuthService authService, IUnitOfWork uow)
    {
        _authService = authService;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _uow.Users.CheckEmailAsync(request.Email);

        if (emailExists)
            throw new InvalidOperationException("Email already exists");

        var passwordHash = _authService.ComputeSha256Hash(request.Password);

        var user = new User(request.Name, request.Email, request.BirthDate, passwordHash, DEFAULT_ROLE);

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return user.Id;
    }
}

