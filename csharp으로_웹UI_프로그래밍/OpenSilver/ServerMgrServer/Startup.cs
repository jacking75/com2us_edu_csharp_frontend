using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServerMsgServer
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
            services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowSpecificOrigins",
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:55591",
                                                          "http://www.contoso.com");
                                  });
            });


            services.AddControllers()
            // 아래 옵션에 의해 답변의 json이 파스칼 형식으로
            //https://docs.microsoft.com/ko-kr/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0
            .AddJsonOptions(options =>
               options.JsonSerializerOptions.PropertyNamingPolicy = null);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("MyAllowSpecificOrigins");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            
        }
    }
}