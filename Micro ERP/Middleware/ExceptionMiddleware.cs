using System.Text.Json;
using MicroERP.Application.Common.Exceptions;

namespace MicroERP.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { message = ex.Message }));
            }
            catch (BusinessException ex)
            {
                context.Response.StatusCode = 400;

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { message = ex.Message }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}