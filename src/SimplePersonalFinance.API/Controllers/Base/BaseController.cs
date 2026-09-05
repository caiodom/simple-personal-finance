using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Core.Domain.Exceptions;

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

    protected void ValidateIds(params Guid[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            const string message = "No IDs provided for validation";
            _logger.LogWarning(message);
            throw new ValidationException(message);
        }

        foreach (var id in ids)
        {
            if (id != Guid.Empty)
                continue;

            var message = $"Invalid ID provided: {id}";
            _logger.LogWarning(message);
            throw new ValidationException(message);
        }
    }

    protected Guid GetUserId()
    {
        var userId = _authUserHandler.GetUserId();
        if (userId != Guid.Empty)
            return userId;

        const string message = "User ID is empty";
        _logger.LogWarning(message);
        throw new DomainException(message);
    }
}
