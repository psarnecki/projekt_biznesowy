using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Tripder.Api.ExceptionHandlers;

// Translates domain/application exceptions to appropriate HTTP responses.
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext ctx,
        Exception exception,
        CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest,
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            InvalidOperationException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(new { status, title }, ct);
        return true;
    }
}
