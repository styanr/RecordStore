using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RecordStore.Api.Filters;

public class PostgresExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception.Message.Contains("42501"))
        {
            context.Result = new ObjectResult(new { message = "You do not have permission to perform this action." })
            {
                StatusCode = 403
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception.Message.Contains("P0001"))
        {
            context.Result = new ObjectResult(new { message = "Must have products in cart to place order." })
            {
                StatusCode = 400
            };
        }
        else if (context.Exception.Message.Contains("P0002"))
        {
            context.Result = new ObjectResult(new { message = "Product is out of stock." })
            {
                StatusCode = 400
            };
        }
        /*else
        {
            context.Result = new ObjectResult(new { message = "An error occurred." })
            {
                StatusCode = 500
            };
        }*/
        context.ExceptionHandled = true;
    }
}