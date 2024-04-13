using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecordStore.Api.Exceptions;

namespace RecordStore.Api.Filters;

public class UnauthorizedExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is UnauthorizedException)
        {
            context.Result = new ObjectResult(new { message = context.Exception.Message })
            {
                StatusCode = 401
            };
            context.ExceptionHandled = true;
        }
    }
}