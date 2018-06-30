using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HybridServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加跨域支持
            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    builder.AllowAnyOrigin()//允许任何来源的主机访问
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddTestUsers(Config.GetTestUsers().ToList())
                    .AddInMemoryIdentityResources(Config.GetIdentityResources());

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("default");
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseMvc();
        }
    }
}
