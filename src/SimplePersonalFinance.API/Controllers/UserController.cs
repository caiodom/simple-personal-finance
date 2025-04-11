using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Application.Commands.CreateUser;
using SimplePersonalFinance.Application.Commands.LoginUser;
using SimplePersonalFinance.Application.Queries.GetUser;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/users")]
[Authorize]
public class UserController(IMediator mediator):ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserQuery(id);

        var user = await mediator.Send(query);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command)
    {
        var result = await mediator.Send(command);

        if (result == Guid.Empty)
            return BadRequest("Failed to create user");

        return CreatedAtAction(nameof(GetById), new { id = result }, command);
    }



    [HttpPut("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var loginUserviewModel = await mediator.Send(command);

        if (loginUserviewModel == null)
            return BadRequest();

        return Ok(loginUserviewModel);
    }
}
