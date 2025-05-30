﻿using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Users;

namespace SimplePersonalFinance.Application.Queries.UserQueries.GetUser
{
    public class GetUserQuery : IRequest<ResultViewModel<UserViewModel>>
    {
        public Guid Id { get; private set; }
        public GetUserQuery(Guid id)
        {
            Id = id;
        }

       
    }
}
