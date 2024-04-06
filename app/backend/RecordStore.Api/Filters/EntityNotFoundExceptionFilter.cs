using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecordStore.Api.Exceptions;

namespace RecordStore.Api.Filters;

public class EntityNotFoundExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is EntityNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new { message = context.Exception.Message });
            context.ExceptionHandled = true;
        }
    }
}