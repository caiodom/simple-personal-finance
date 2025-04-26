using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.API.Controllers.Base;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAuthUserHandler _authUserHandler;

    protected BaseController(ILogger logger, IAuthUserHandler authUserHandler)
    {
        _logger = logger;
        _authUserHandler = authUserHandler;
    }

    protected IActionResult HandleResult(ResultViewModel result)
    {
        if (result == null)
        {
            _logger.LogWarning("Result was null when handling response");
            throw new DomainException("Invalid response received");
        }

        _logger.LogInformation("Request succeeded: {Message}", result.Message);
        return Ok(result);
    }

    protected IActionResult CreatedResult<T>(string actionName, object routeValues, ResultViewModel<T> result)
    {
        if (result == null)
        {
            _logger.LogWarning("Result was null when handling creation response");
            throw new DomainException("Invalid response received");
        }

        _logger.LogInformation("Resource created successfully: {Message}", result.Message);
        return CreatedAtAction(actionName, routeValues, result);
    }

    protected void ValidateIds(params Guid[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            var message = "No IDs provided for validation";
            _logger.LogWarning(message);
            throw new ValidationException(message);
        }

        foreach (var id in ids)
        {
            if (id == Guid.Empty)
            {
                var message = $"Invalid ID provided: {id}";
                _logger.LogWarning(message);
                throw new ValidationException(message);
            }
        }
    }

    protected Guid GetUserId()
    {
        Guid userId = _authUserHandler.GetUserId();
        if (userId == Guid.Empty)
        {
            var message = "User ID is empty";
            _logger.LogWarning(message);
            throw new DomainException(message);
        }

        return userId;
    }
}
