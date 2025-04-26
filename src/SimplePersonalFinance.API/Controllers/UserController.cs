using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Requests.UserRequests;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;
using SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;
using SimplePersonalFinance.Application.Queries.UserQueries.GetUser;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/users")]
[Authorize]
public class UserController(IMediator mediator, IAuthUserHandler authUserHandler,ILogger<UserController> logger):BaseController(logger, authUserHandler)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        ValidateIds(id);
        var query = new GetUserQuery(id);
        var result = await mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Post(CreateUserRequest request)
    {
        var command= new CreateUserCommand(request.Name,request.Password,request.Email,request.BirthDate);
        var result = await mediator.Send(command);

        return HandleResult(result);
    }

    [HttpPut("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var command= new LoginUserCommand(request.Email, request.Password);
        var result = await mediator.Send(command);

        return HandleResult(result);
    }
}
