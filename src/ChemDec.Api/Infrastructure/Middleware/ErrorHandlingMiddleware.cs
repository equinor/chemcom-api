using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            /*     if (exception is ValidationException) code = HttpStatusCode.NotFound;
                  else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
                  else if (exception is MyException) code = HttpStatusCode.BadRequest;*/

            var t = exception.GetType();
            var actualException = exception;
            while (actualException.InnerException != null) actualException = actualException.InnerException;

            var result = JsonConvert.SerializeObject(new { error = actualException.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
