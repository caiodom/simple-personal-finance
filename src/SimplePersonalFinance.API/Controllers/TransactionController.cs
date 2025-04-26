using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Controllers.Base;
using SimplePersonalFinance.API.Requests.TransactionRequests;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;
using SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

namespace SimplePersonalFinance.API.Controllers;

[Route("api/transactions")]
[Authorize]
public class TransactionController(IMediator mediator, IAuthUserHandler authUserHandler,ILogger<TransactionController> logger):BaseController(logger, authUserHandler)
{

    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery]GetTransactionsRequest request)
    {
        var result = await mediator.Send(new GetTransactionsQuery(request.AccountId, request.PageNumber, request.PageSize));

        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionsById(Guid id)
    {
        var result = await mediator.Send(new GetTransactionByIdQuery(id));

        return HandleResult(result);

    }

}
