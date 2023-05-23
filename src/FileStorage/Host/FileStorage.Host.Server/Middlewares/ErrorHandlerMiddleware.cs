using AutoMapper.Internal;
using FileStorage.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.Json;

namespace FileStorage.Host.Middlewares
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
                var error = new ErrorDto
                {
                    TraceId = context.TraceIdentifier,
                    Message = exception?.Message
                };


                switch (exception)
                {
                    case KeyNotFoundException e:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    case ArgumentException e:
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        break;
                    case ValidationException e:
                        error.Message = "Ошибка валидации: ";
                        e.Errors.ForAll(a => error.Message += " -" + a.ErrorMessage);
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        break;
                    case UnauthorizedAccessException e:
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        _logger.LogCritical(exception, exception?.Message);
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
                        return;
                }
               
                _logger.LogError(exception, exception?.Message);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
            }
        }
    }
}
