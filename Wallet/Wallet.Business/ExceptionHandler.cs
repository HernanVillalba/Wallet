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
                await _next(httpContext);
                int statusCode = httpContext.Response.StatusCode;
                if (statusCode >= 300)
                {
                    // The response was an error. We must show it properly

                    throw new CustomException(statusCode, GetErrorString(statusCode));
                }
            }
            catch (CustomException ex)
            {
                await HandleCustomExceptionAsync(httpContext, ex);
            }
            catch
            {
                await HandleExceptionAsync(httpContext);
            }
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

        string GetErrorString(int statusCode)
        {
            switch(statusCode)
            {
                case 400:
                    return "Datos de entrada inválidos";
                case 401:
                    return "Acceso no autorizado";
                default:
                    return "Error interno del servidor";
            }
        }
    }
}
