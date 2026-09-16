using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Middlewares
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IErrorLogsProvider _errorLogsProvider;

        public ErrorLoggingMiddleware(RequestDelegate next, IErrorLogsProvider errorLogsProvider)
        {
            _next = next;
            _errorLogsProvider = errorLogsProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _errorLogsProvider.AddErrorLog(new ErrorInfo
                {
                    DateTime = DateTime.Now,
                    Message = e.Message,
                    StackTrace = e.ToString(),
                    RequestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}",
                    RequestHeaders = context.Request.Headers.ToString()
                });
            }
        }
    }
}
