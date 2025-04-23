using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;

namespace SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;

public class LoginUserCommand : IRequest<ResultViewModel<LoginUserViewModel>>
{
    public string Email { get;}
    public string Password { get;}

    public LoginUserCommand(string email,string password)
    {
        Email = email;
        Password = password;
    }
}
