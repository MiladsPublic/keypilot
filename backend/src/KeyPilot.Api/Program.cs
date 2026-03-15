using KeyPilot.Api.Endpoints.Properties;
using KeyPilot.Api.Extensions;
using KeyPilot.Application;
using KeyPilot.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

var propertyGroup = app.MapGroup("/api/v1/properties").WithTags("Properties");
CreatePropertyEndpoint.Map(propertyGroup);
GetPropertyEndpoint.Map(propertyGroup);

app.Run();

public partial class Program;
