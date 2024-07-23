using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;
namespace BackendApi_mvc;
//kestrel
// dont change this if u dont know how


public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        //this is a work of art like like allowing async funtions is like good
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // // Add support for async calls
        // app.Run(async context =>
        // {
        //     await context.Response.WriteAsync("Hello, World!");
        // });
    }
}
