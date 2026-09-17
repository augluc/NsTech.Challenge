namespace NsTech.Challenge.Api.Middlewares;

using Microsoft.AspNetCore.Mvc;
using NsTech.Challenge.Domain.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail) = exception switch
        {
            InsufficientStockException ex => (StatusCodes.Status400BadRequest, "Insufficient Stock", ex.Message),
            InvalidOrderStateException ex => (StatusCodes.Status422UnprocessableEntity, "Invalid Order State", ex.Message),
            DomainException ex => (StatusCodes.Status400BadRequest, "Domain Error", ex.Message),
            KeyNotFoundException ex => (StatusCodes.Status404NotFound, "Resource Not Found", ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}