using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserViewModel>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _uow;
    public LoginUserCommandHandler(IAuthService authService, IUnitOfWork uow)
    {
        _authService = authService;
        _uow = uow;
    }

    public async Task<LoginUserViewModel> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // Utilizar o mesmo algoritmo para criar o hash dessa senha
        var passwordHash = _authService.ComputeSha256Hash(request.Password);

        // Buscar no meu banco de dados um User que tenha meu e-mail e minha senha em formato hash
        var user = await _uow.Users.GetUserByEmailAndPasswordAsync(request.Email, passwordHash);

        // Se nao existir, erro no login
        if (user == null)
            return null;

        // Se existir, gero o token usando os dados do usuário
        var token = _authService.GenerateJwtToken(user.Email, user.Role);

        return new LoginUserViewModel(user.Email, token);
    }
}
