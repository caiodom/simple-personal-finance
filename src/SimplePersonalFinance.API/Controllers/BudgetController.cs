using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Requests.BudgetRequests;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudget;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/budgets")]
[Authorize]
public class BudgetController : BaseController
{
    private readonly IMediator _mediator;

    public BudgetController(IMediator mediator,
                            IAuthUserHandler authUserHandler,
                            ILogger<BudgetController> logger)
        : base(logger, authUserHandler)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Guid userId = GetUserId();
        var result = await _mediator.Send(new GetBudgetsQuery(userId));
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBudgetByIdQuery(id));
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(CreateBudgetRequest request)
    {
        Guid userId = GetUserId();
        var command = new CreateBudgetCommand(userId,request.Category, request.LimitAmount, request.Month, request.Year);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditBudget(Guid id, EditBudgetRequest request)
    {
        ValidateIds(id);

        var command = new EditBudgetCommand(id, request.LimitAmount, request.Month, request.Year);
        var result = await _mediator.Send(command);

        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        var result = await _mediator.Send(new RemoveBudgetCommand(id));

        return HandleResult(result);
    }
}
