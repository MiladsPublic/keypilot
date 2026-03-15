using KeyPilot.Api.Endpoints.Conditions;
using KeyPilot.Api.Endpoints.Properties;
using KeyPilot.Api.Endpoints.Tasks;
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
SettlePropertyEndpoint.Map(propertyGroup);

var taskGroup = app.MapGroup("/api/v1/tasks").WithTags("Tasks");
CompleteTaskEndpoint.Map(taskGroup);

var conditionGroup = app.MapGroup("/api/v1/conditions").WithTags("Conditions");
CompleteConditionEndpoint.Map(conditionGroup);

app.Run();

public partial class Program;
