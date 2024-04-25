using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecordStore.Api.Filters;

public class GenericExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(new { message = "An error occurred while processing your request." })
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
        base.OnException(context);
    }
}