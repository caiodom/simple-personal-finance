using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ResultViewModel<LoginUserViewModel>>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _uow;
    public LoginUserCommandHandler(IAuthService authService, IUnitOfWork uow)
    {
        _authService = authService;
        _uow = uow;
    }

    public async Task<ResultViewModel<LoginUserViewModel>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {

        var passwordHash = _authService.ComputeSha256Hash(request.Password);

        var user = await _uow.Users.GetUserByEmailAndPasswordAsync(request.Email, passwordHash);


        if (user is null)
            throw new BusinessRuleViolationException("Invalid Credentials",
                "The email or password provided is incorrect. Please try again.");


        var token = _authService.GenerateJwtToken(user.Id, user.Email.Value, user.Role);

        return ResultViewModel<LoginUserViewModel>.Success(new LoginUserViewModel(user.Email.Value, token));
    }
}
