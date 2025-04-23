using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Application.ViewModels;
using System.Diagnostics;

namespace SimplePersonalFinance.API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        private readonly ILogger _logger;
        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleResult<T>(ResultViewModel<T> result)
        {
            if (result == null)
            {
                _logger.LogWarning("Result was null when handling typed response");
                return BadRequest("Invalid response received");
            }

            return result.ResponseType switch
            {
                ResponseType.Ok => OkResultHandler(result),
                ResponseType.NotFound => NotFoundResultHandler(result),
                ResponseType.Error => BadRequestResultHandler(result),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(result.ResponseType),
                    $"Unexpected response type: {result.ResponseType}")
            };
        }

        protected IActionResult HandleResult(ResultViewModel result)
        {
            if (result == null)
            {
                _logger.LogWarning("Result was null when handling response");
                return BadRequest("Invalid response received");
            }

            return result.ResponseType switch
            {
                ResponseType.Ok => OkResultHandler(result),
                ResponseType.NotFound => NotFoundResultHandler(result),
                ResponseType.Error => BadRequestResultHandler(result),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(result.ResponseType),
                    $"Unexpected response type: {result.ResponseType}")
            };
        }

        protected IActionResult CreatedResult<T>(string actionName, object routeValues, ResultViewModel<T> result)
        {
            if (!result.IsSuccess)
            {
                return HandleResult(result);
            }

            _logger.LogInformation("Resource created successfully: {Message}", result.Message);
            return CreatedAtAction(actionName, routeValues, result);
        }

        private OkObjectResult OkResultHandler<T>(ResultViewModel<T> result)
        {
            _logger.LogInformation("Request succeeded: {Message}", result.Message);
            return Ok(result);
        }

        private OkObjectResult OkResultHandler(ResultViewModel result)
        {
            _logger.LogInformation("Request succeeded: {Message}", result.Message);
            return Ok(result);
        }

        private NotFoundObjectResult NotFoundResultHandler(ResultViewModel result) 
        {
            _logger.LogWarning("Resource not found: {Message}", result.Message);
            return NotFound(result);
        }

        private BadRequestObjectResult BadRequestResultHandler(ResultViewModel result)
        {
            _logger.LogError("Request failed: {Message}", result.Message);
            return BadRequest(result);
        }


    }
}

