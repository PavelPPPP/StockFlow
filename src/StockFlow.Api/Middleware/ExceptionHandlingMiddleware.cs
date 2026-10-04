using StockFlow.Application.Common.Exceptions;
using StockFlow.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace StockFlow.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Validation faild", ex.Errors);
            }
            catch (DomainException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Business rule violation", ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, "Invalid argument", ex.Message);
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string title, object detail)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new { title, detail };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}