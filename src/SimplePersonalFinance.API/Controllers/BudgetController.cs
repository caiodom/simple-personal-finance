using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/budget")]
[Authorize]
public class BudgetController(IMediator mediator, IAuthUserHandler authUserHandler,ILogger<BudgetController> logger) : BaseController(logger)
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Guid userId = authUserHandler.GetUserId();
        var result = await mediator.Send(new GetBudgetsQuery(userId));
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetBudgetByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        Guid userId = authUserHandler.GetUserId();
        command.SetUserId(userId);
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditBudget([FromBody] EditBudgetCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        var result = await mediator.Send(new RemoveBudgetCommand(id));
        return HandleResult(result);
    }


}
