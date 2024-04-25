using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecordStore.Api.Filters;

public class InvalidOperationExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is InvalidOperationException)
        {
            context.Result = new ObjectResult(new { message = context.Exception.Message }) 
            {
                StatusCode = 400
            };
            context.ExceptionHandled = true;
        }
        base.OnException(context);
    }
}