using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;
using Tripder.Application;
using Tripder.Infrastructure;
using Tripder.Infrastructure.Persistence;
using Tripder.Infrastructure.Persistence.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

/// <summary>
/// Translates domain/application exceptions to appropriate HTTP responses.
/// </summary>
internal sealed class GlobalExceptionHandler : IExceptionHandler
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