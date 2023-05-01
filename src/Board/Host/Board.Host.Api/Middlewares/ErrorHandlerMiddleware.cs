using System;
using System.Net;
using System.Text.Json;

namespace Board.Host.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case KeyNotFoundException e:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            traceId = context.TraceIdentifier,
                            message = e.Message
                        }));
                        break;
                    case ArgumentException e:
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
