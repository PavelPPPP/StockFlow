using StockFlow.Application.Common.Exceptions;
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
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = new { title = "Validation faild", error = ex.Errors };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}