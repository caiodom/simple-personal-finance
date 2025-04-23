using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Commands.CreateAccount;
using SimplePersonalFinance.Application.Commands.CreateTransaction;
using SimplePersonalFinance.Application.Commands.EditTransaction;
using SimplePersonalFinance.Application.Commands.RemoveAccount;
using SimplePersonalFinance.Application.Commands.RemoveTransaction;
using SimplePersonalFinance.Application.Queries.GetAccount;
using SimplePersonalFinance.Application.Queries.GetAccountsByUserId;
using SimplePersonalFinance.Application.Queries.GetAccountTransactions;
using System.Security.Claims;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/account")]
[Authorize]
public class AccountController(IMediator mediator,IAuthUserHandler authUserHandler) : ControllerBase
{

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAccountByIdQuery(id));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetByUserId()
    {
        Guid userId=Guid.Empty;

        if (HttpContext.User.Identity?.IsAuthenticated == true)
              userId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        var result = await mediator.Send(new GetAccountByUserIdQuery(userId));
        if (!result.IsSuccess)
            return BadRequest(result.Message);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]CreateAccountCommand command)
    {
        Guid userId = authUserHandler.GetUserId();

        if(userId == Guid.Empty)
            return BadRequest("User not found");

        command.SetUserId(userId);

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, command);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new RemoveAccountCommand(id));

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return NoContent();
    }

    //REMOVE
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
