using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;
using SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;
using SimplePersonalFinance.Application.Queries.UserQueries.GetUser;
using SimplePersonalFinance.Core.Domain.Results;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/users")]
[Authorize]
public class UserController(IMediator mediator,ILogger<UserController> logger):BaseController(logger)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserQuery(id);
        var result = await mediator.Send(query);
        return HandleResult(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }
}
