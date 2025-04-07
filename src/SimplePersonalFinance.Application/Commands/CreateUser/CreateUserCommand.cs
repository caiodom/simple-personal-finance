using MediatR;

namespace SimplePersonalFinance.Application.Commands.CreateUser;

public class CreateUserCommand:IRequest<Guid>
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
}
