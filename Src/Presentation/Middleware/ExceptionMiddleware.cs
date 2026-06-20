using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.MIddleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
            Polly.CircuitBreaker.BrokenCircuitException => (int)HttpStatusCode.ServiceUnavailable,
            HttpRequestException => (int)HttpStatusCode.ServiceUnavailable,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var title = statusCode switch
        {
            (int)HttpStatusCode.NotFound => "Recurso no encontrado",
            (int)HttpStatusCode.BadRequest => "Operacion invalida",
            (int)HttpStatusCode.Forbidden => "Acceso denegado",
            (int)HttpStatusCode.ServiceUnavailable => "Servicio no disponible",
            _ => "Error interno del servidor"
        };

        var detail = statusCode switch
        {
            (int)HttpStatusCode.InternalServerError => "Ocurrio un error inesperado. Intenta nuevamente o contacta a soporte.",
            (int)HttpStatusCode.ServiceUnavailable => "Error de comunicación con el servicio de Inventario. El circuito está abierto o el servicio no responde.",
            _ => exception.Message
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}