using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Application.Commands.CreateTransaction;
using SimplePersonalFinance.Application.Commands.EditTransaction;
using SimplePersonalFinance.Application.Commands.RemoveTransaction;
using SimplePersonalFinance.Application.Queries.GetAccount;
using SimplePersonalFinance.Application.Queries.GetAccountsByUserId;
using SimplePersonalFinance.Application.Queries.GetAccountTransactions;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/account")]
//[Authorize]
public class AccountController(IMediator mediator) : ControllerBase
{

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAccountByIdQuery(id));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    //get accounts by userId create.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await mediator.Send(new GetAccountByUserIdQuery(userId));
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]CreateAccountCommand command)
    {
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, command);
    }


    [HttpGet("transactions/{accountId}")]
    public async Task<IActionResult> GetTransactions(Guid accountId)
    {
        var result = await mediator.Send(new GetAccountTransactionsQuery(accountId));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPost("transactions")]
    public async Task<IActionResult> PostTransaction([FromBody] CreateAccountTransactionCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetTransactions), new { accountId = command.AccountId }, command);
    }

    [HttpPut("transactions")]
    public async Task<IActionResult> PutTransaction([FromBody] EditAccountTransactionCommand command)
    {
        var result = await mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetTransactions), new { accountId = command.AccountId }, command);
    }

    [HttpDelete("{accountId}/transactions/{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid accountId, Guid transactionId)
    {
        var result = await mediator.Send(new DeleteAccountTransactionCommand(transactionId, accountId));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return NoContent();
    }
}
