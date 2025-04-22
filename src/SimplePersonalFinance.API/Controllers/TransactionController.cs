using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Application.Queries.GetTransactionById;
using SimplePersonalFinance.Application.Queries.GetTransactions;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/transaction")]
[Authorize]
public class TransactionController(IMediator mediator):ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] Guid accountId, 
                                                     [FromQuery] int pageNumber = 1, 
                                                     [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetTransactionsQuery(accountId, pageNumber, pageSize));

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionsById(Guid id)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id));

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result);
    }

}
