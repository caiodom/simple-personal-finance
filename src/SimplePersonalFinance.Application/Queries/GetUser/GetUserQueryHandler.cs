using MediatR;
using SimplePersonalFinance.Application.ViewModels.Users;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.GetUser;

public class GetUserQueryHandler(IUnitOfWork uow) : IRequestHandler<GetUserQuery, UserViewModel>
{
    public async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user=await uow.Users.GetByIdAsync(request.Id);

        if (user == null)
            return null;

        return new UserViewModel(user.Name, user.Email);
    }
}
