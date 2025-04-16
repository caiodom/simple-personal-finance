using MediatR;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.Application.Commands.CreateUser;

public class CreateUserCommand:IRequest<ResultViewModel<Guid>>
{
    public string Name { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public DateTime BirthDate { get; private set; }

    public CreateUserCommand(string name, string password,string email, DateTime birthDate)
    {
        Name=name;
        Password = password;
        Email = email;
        BirthDate = birthDate;

    }
}
