using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.UserQueries.GetUser;

public class GetUserQueryHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<GetUserQuery, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (request.Id != currentUser.UserId)
            throw new EntityNotFoundException("User", request.Id);

        var user = await uow.Users.GetByIdAsync(request.Id);
        if (user == null)
            throw new EntityNotFoundException("User", request.Id);

        return ResultViewModel<UserViewModel>.Success(new UserViewModel(user.Name, user.Email.Value));
    }
}
