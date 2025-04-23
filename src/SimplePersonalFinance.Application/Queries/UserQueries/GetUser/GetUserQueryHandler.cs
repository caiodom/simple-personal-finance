using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.UserQueries.GetUser;

public class GetUserQueryHandler(IUnitOfWork uow) : IRequestHandler<GetUserQuery, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await uow.Users.GetByIdAsync(request.Id);
        if (user == null)
            return ResultViewModel<UserViewModel>.NotFound("User not found");

        return ResultViewModel<UserViewModel>.Success(new UserViewModel(user.Name, user.Email.Value));
    }
}
