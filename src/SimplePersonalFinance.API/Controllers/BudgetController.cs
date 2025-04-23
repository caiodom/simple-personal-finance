using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/budget")]
[Authorize]
public class BudgetController(IMediator mediator, IAuthUserHandler authUserHandler) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Guid userId = authUserHandler.GetUserId();

        var result = await mediator.Send(new GetBudgetsQuery(userId));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetBudgetByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        Guid userId = authUserHandler.GetUserId();

        command.SetUserId(userId);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { Id = result.Data }, command);
    }

    [HttpPut]
    public async Task<IActionResult> EditBudget([FromBody] EditBudgetCommand command)
    {
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { Id = result.Data }, command);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        var result = await mediator.Send(new RemoveBudgetCommand(id));

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return NoContent();
    }


}
