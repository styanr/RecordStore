using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace RecordStore.Api.Filters;

public class PostgresExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is PostgresException postgresException)
        {
            HandlePostgresException(context, postgresException);
        }
        if (context.Exception is DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException is PostgresException innerPostgresException)
            {
                HandlePostgresException(context, innerPostgresException);
            }
        }
    }

    private void HandlePostgresException(ExceptionContext context, PostgresException postgresException)
    {
        context.Result = postgresException.SqlState switch
        {
            "23505" => new ConflictObjectResult(postgresException.Message),
            "42501" => new ObjectResult(new { message = "You do not have permission to perform this action." })
                { StatusCode = 403 },
            "P0001" => new ObjectResult(new { message = "Must have products in cart to place order." })
                { StatusCode = 400 },
            "P0002" => new ObjectResult(new { message = "Product is out of stock." }) { StatusCode = 400 },
            _ => new BadRequestObjectResult(postgresException.Message)
        };

        context.ExceptionHandled = true;
    }
}