using MediatR;
using SimplePersonalFinance.Application.ViewModels.Users;

namespace SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;

public class LoginUserCommand : IRequest<LoginUserViewModel>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
