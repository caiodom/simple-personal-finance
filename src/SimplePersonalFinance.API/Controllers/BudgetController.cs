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

    public BudgetController(
        IMediator mediator,
        IAuthUserHandler authUserHandler,
        ILogger<BudgetController> logger)
        : base(logger, authUserHandler)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetBudgetsQuery(userId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBudgetByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(CreateBudgetRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new CreateBudgetCommand(userId, request.Category, request.LimitAmount, request.Month, request.Year);
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditBudget(Guid id, EditBudgetRequest request, CancellationToken cancellationToken)
    {
        ValidateIds(id);
        var command = new EditBudgetCommand(id, request.LimitAmount, request.Month, request.Year);
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveBudgetCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
