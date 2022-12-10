using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using signalR_server.Hubs;

namespace signalR_server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // setup which clients can access the server
            services.AddCors(options => options.AddDefaultPolicy(policy =>
                policy.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)
            ));
            services.AddSignalR(); // activate signalR
            //services.AddControllers(); // active the controllers
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(); 
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // https://localhost:5001/myhub
                endpoints.MapHub<MyHub>("/myhub");
                //endpoints.MapHub<MessageHub>("/messagehub");
                //endpoints.MapControllers();
            });
        }
    }
}
