using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});


builder.Services.ConfigureServices(builder.Configuration);



var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

if(!builder.Environment.IsEnvironment("Test"))
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot/", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles(); 

app.UseRouting(); //Identifying action method based route
app.UseAuthentication(); //Reading Identity cookie
app.UseAuthorization();
app.MapControllers(); //Execute the filter pipeline (action+filters)

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}"); //Home and Index are default value if you don't specify any, not constant route
}); //Conventionnal routing used to make a route pattern instead of declaring an attribute routing for each action in controller


app.Run();

app.UseHttpLogging();

public partial class Program { }