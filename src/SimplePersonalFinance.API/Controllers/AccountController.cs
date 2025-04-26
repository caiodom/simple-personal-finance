using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Requests.AccountRequests;
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
public class AccountController(IMediator mediator, IAuthUserHandler authUserHandler, ILogger<AccountController> logger) : BaseController(logger, authUserHandler)
{

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAccountByIdQuery(id));
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByUserId([FromQuery]GetByUserIdRequest request)
    {
        var userId = GetUserId();
        var query = new GetAccountByUserIdQuery(userId, request.PageNumber, request.PageSize);
        var result = await mediator.Send(query);

        return HandleResult(result);

    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateAccountRequest request)
    {
        var userId = GetUserId();
        var command = new CreateAccountCommand(userId, request.AccountType, request.Name, request.InitialBalance);
        var result = await mediator.Send(command);

        return HandleResult(result);

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        ValidateIds(id);
        var result = await mediator.Send(new RemoveAccountCommand(id));
        return HandleResult(result);
    }

    [HttpGet("{id}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid id)
    {
        ValidateIds(id);
        var result = await mediator.Send(new GetAccountTransactionsQuery(id));
        return HandleResult(result);
    }

    [HttpPost("{id}/transactions")]
    public async Task<IActionResult> PostTransaction(Guid id, CreateAccountTransactionRequest request)
    {
        ValidateIds(id);
        var command = new CreateAccountTransactionCommand(id,
                                                          request.CategoryId,
                                                          request.TransactionTypeId,
                                                          request.Description,
                                                          request.Amount,
                                                          request.Date);


        var result = await mediator.Send(command);

        return HandleResult(result);
    }

    [HttpPut("{id}/transactions/{transactionId}")]
    public async Task<IActionResult> PutTransaction(Guid id, Guid transactionId, EditAccountTransactionRequest request)
    {
        ValidateIds(id, transactionId);

        var command = new EditAccountTransactionCommand(transactionId, id, request.Amount, request.Description, request.CategoryId, request.TransactionTypeId);
        var result = await mediator.Send(command);

        return HandleResult(result);
    }

    [HttpDelete("{id}/transactions/{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid id, Guid transactionId)
    {
        ValidateIds(id, transactionId);
        var result = await mediator.Send(new DeleteAccountTransactionCommand(transactionId, id));
        return HandleResult(result);
    }
}
