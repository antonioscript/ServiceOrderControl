
using OsService.Application; 
using OsService.Infrastructure;                      
using OsService.ServiceDefaults.DependencyInjection; 


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services.AddControllers();

//Módules
builder.Services.AddModules(builder.Configuration,
    typeof(ApplicationModule).Assembly,
    typeof(InfrastructureModule).Assembly);


// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var generator = scope.ServiceProvider.GetRequiredService<OsService.Infrastructure.Databases.DatabaseGenerantor>();
    await generator.EnsureCreatedAsync(CancellationToken.None);
}


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}


app.MapDefaultEndpoints();
app.MapControllers();


app.Run();
