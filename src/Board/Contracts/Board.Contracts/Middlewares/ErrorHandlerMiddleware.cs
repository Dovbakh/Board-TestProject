using Board.Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;

namespace Board.Contracts.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception exception)
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    traceId = context.TraceIdentifier,
                    message = exception?.Message
                });

                switch (exception)
                {
                    case KeyNotFoundException e:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    case ArgumentException e:
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        break;
                    case UnauthorizedAccessException e:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;
                    case ForbiddenException e:
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        _logger.LogCritical(exception, exception?.Message);
                        await context.Response.WriteAsync(result);
                        return;
                }
               
                _logger.LogError(exception, exception?.Message);
                await context.Response.WriteAsync(result);
            }
        }
    }
}
