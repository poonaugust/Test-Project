using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using TestProject.Application;
using TestProject.Infrastructure;
using TestProject.Infrastructure.Persistence;
using TestProject.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestProject Clean Architecture API",
        Version = "v1",
        Description = "ASP.NET Core (.NET 10) API built with Clean Architecture, CQRS, Mediator, SOLID, EF Core, and PostgreSQL."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

var autoMigrate = app.Configuration.GetValue<bool>("Database:AutoMigrate");
if (autoMigrate || app.Environment.IsDevelopment())
{
    await DbInitializer.InitializeAsync(app.Services, autoMigrate: true);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestProject API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors("DefaultCors");
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
