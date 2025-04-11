using MediatR;
using SimplePersonalFinance.Application.ViewModels.Users;

namespace SimplePersonalFinance.Application.Commands.LoginUser;

public class LoginUserCommand : IRequest<LoginUserViewModel>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
