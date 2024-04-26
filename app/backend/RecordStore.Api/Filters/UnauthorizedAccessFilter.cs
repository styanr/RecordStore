using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecordStore.Api.Filters;

public class UnauthorizedAccessFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is UnauthorizedAccessException)
        {
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = 403
            };
            context.ExceptionHandled = true;
        }
    }
}