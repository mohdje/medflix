﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Middlewares
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private string _loggingFile = Path.Combine(AppFolders.StorageFolder, "error.txt");

        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(_loggingFile, true))
                {
                    await sw.WriteAsync(BuildErrorMessage(e, context));
                }
            }
        }

        private string BuildErrorMessage(Exception e, HttpContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(Environment.NewLine);

            stringBuilder.Append($"########################### Exception ##################################");

            stringBuilder.Append(Environment.NewLine);

            stringBuilder.Append($"-{DateTime.Now.ToString("MM/dd/yyyy H:mm")} : ");

            stringBuilder.Append(Environment.NewLine);

            stringBuilder.Append($"ERROR = {e.ToString()}");

            stringBuilder.Append(Environment.NewLine);

            stringBuilder.Append($"CONTEXT = ");

            if(context?.Request?.QueryString != null)
                stringBuilder.Append($"query: {context.Request.QueryString}");

            stringBuilder.Append(Environment.NewLine);

            stringBuilder.Append($"########################### End of Exception ##################################");


            return stringBuilder.ToString();
        }
    }
}
