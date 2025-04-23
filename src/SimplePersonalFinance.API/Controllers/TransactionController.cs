using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;
using SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/transactions")]
[Authorize]
public class TransactionController(IMediator mediator,ILogger<TransactionController> logger):BaseController(logger)
{

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] Guid accountId, 
                                                     [FromQuery] int pageNumber = 1, 
                                                     [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetTransactionsQuery(accountId, pageNumber, pageSize));
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionsById(Guid id)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id));
        return HandleResult(result);
    }

}
