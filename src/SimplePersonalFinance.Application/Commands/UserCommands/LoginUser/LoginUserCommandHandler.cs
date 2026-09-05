using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;

public class LoginUserCommandHandler(
    IAuthService authService,
    IUserRepository users) : IRequestHandler<LoginUserCommand, ResultViewModel<LoginUserViewModel>>
{
    public async Task<ResultViewModel<LoginUserViewModel>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !authService.VerifyPassword(request.Password, user.PasswordHash))
            throw new BusinessRuleViolationException(
                "Invalid Credentials",
                "The email or password provided is incorrect. Please try again.");

        var token = authService.GenerateJwtToken(user.Id, user.Email.Value, user.Role);

        return ResultViewModel<LoginUserViewModel>.Success(new LoginUserViewModel(user.Email.Value, token));
    }
}
