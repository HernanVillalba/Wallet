using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wallet.Entities;

namespace Wallet.Business
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Execute the HTTP request
                await _next(httpContext);

                // Check the response status code
                int statusCode = httpContext.Response.StatusCode;
                if (statusCode == 401) // Special case when you are not logged in
                {
                    throw new CustomException(statusCode, "Acceso no autorizado");
                }
            }
            catch (CustomException ex)
            {
                // Custom exception handler (to handle common errors thrown in business)
                await HandleCustomExceptionAsync(httpContext, ex);
            }
            catch
            {
                // Internal exception handler
                await HandleExceptionAsync(httpContext);
            }
        }

        private Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;
            return context.Response.WriteAsync(new ErrorModel()
            {
                status = context.Response.StatusCode,
                errors = new Dictionary<string, List<string>>() {
                    { "error",  new List<string>() { exception.Error } }
                }
            }.ToString());
        }

        private Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(new ErrorModel()
            {
                status = context.Response.StatusCode,
                errors = new Dictionary<string, List<string>>() {
                    { "error",  new List<string>() { "Error interno del servidor" } }
                }
            }.ToString());
        }
    }
}
