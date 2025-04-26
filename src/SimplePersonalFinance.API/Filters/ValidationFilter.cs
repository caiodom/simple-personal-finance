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
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var result = new ResultViewModel(false, "Validation error");

            // Adiciona os erros nas extensions
            result.AddExtension("errors", errors);

            context.Result = new BadRequestObjectResult(result);
        }
    }
}
