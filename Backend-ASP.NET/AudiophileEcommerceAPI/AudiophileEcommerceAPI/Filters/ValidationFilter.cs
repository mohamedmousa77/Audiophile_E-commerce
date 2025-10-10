using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AudiophileEcommerceAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorResponse = new
                {
                    status = 400,
                    title = "Errori di validazione",
                    errors = errors,
                    traceId = context.HttpContext.TraceIdentifier
                };

                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Non serve implementazione
        }
    }

}

