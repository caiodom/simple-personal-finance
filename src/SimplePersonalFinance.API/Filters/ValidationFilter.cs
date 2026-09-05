using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value?.Errors.Select(error => error.ErrorMessage).ToArray()
                    ?? Array.Empty<string>());

        var result = new ResultViewModel(false, "Validation error");
        result.AddExtension("errors", errors);

        context.Result = new BadRequestObjectResult(result);
    }
}
