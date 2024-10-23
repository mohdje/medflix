using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Middlewares;
using WebHostStreaming.StartupExtensions;

namespace WebHostStreaming
{
    public class Startup
    {
        const string ALLOW_SPECIFIC_ORIGIN = "AllowSpecificOrigin";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(ALLOW_SPECIFIC_ORIGIN,
                    builder => builder.WithOrigins("*").AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.AddControllers();

            services.AddProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.SetupUI();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(ALLOW_SPECIFIC_ORIGIN);

            app.UseAuthorization();

            app.UseMiddleware<ErrorLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Movies}/{action=Suggested}/{id?}");
            });
        }
    }
}
