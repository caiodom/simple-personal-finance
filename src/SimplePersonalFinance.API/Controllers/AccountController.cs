using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.AccountCommands.CreateAccount;
using SimplePersonalFinance.Application.Commands.AccountCommands.CreateTransaction;
using SimplePersonalFinance.Application.Commands.AccountCommands.EditTransaction;
using SimplePersonalFinance.Application.Commands.AccountCommands.RemoveAccount;
using SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;
using SimplePersonalFinance.Application.Queries.AccountQueries.GetAccount;
using SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountsByUserId;
using SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountTransactions;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/accounts")]
[Authorize]
public class AccountController(IMediator mediator, IAuthUserHandler authUserHandler, ILogger<AccountController> logger) : BaseController(logger)
{

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAccountByIdQuery(id));
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUserId(int pageNumber = 1, int pageSize = 10)
    {
        Guid userId = authUserHandler.GetUserId();
        var result = await mediator.Send(new GetAccountByUserIdQuery(userId, pageNumber, pageSize));
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountCommand command)
    {
        Guid userId = authUserHandler.GetUserId();

        if (userId == Guid.Empty)
            return BadRequest("User not found");

        command.SetUserId(userId);

        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new RemoveAccountCommand(id));
        return HandleResult(result);
    }

    [HttpGet("{accountId}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid accountId)
    {
        var result = await mediator.Send(new GetAccountTransactionsQuery(accountId));
        return HandleResult(result);
    }

    [HttpPost("transactions")]
    public async Task<IActionResult> PostTransaction([FromBody] CreateAccountTransactionCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPut("transactions")]
    public async Task<IActionResult> PutTransaction([FromBody] EditAccountTransactionCommand command)
    {
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{accountId}/transactions/{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid accountId, Guid transactionId)
    {
        var result = await mediator.Send(new DeleteAccountTransactionCommand(transactionId, accountId));
        return HandleResult(result);
    }
}
