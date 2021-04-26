using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wallet.Business.Exceptions;
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
            }
            catch (InvalidException ex)
            {
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.Forbidden, ex.Message);
            }
            catch (BusinessException ex)
            {
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                // Internal exception handler
                await HandleExceptionAsync(httpContext, (int)HttpStatusCode.InternalServerError, "Error interno del servidor");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            // The problem with this approach is that we ignore any content negotiation as we always return JSON
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(new ErrorModel()
            {
                status = context.Response.StatusCode,
                errors = new Dictionary<string, List<string>>() {
                    { "error",  new List<string>() { message } }
                }
            }.ToString());
        }
    }
}
