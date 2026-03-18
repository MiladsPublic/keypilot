using KeyPilot.Api.Endpoints.Conditions;
using KeyPilot.Api.Endpoints.Config;
using KeyPilot.Api.Endpoints.Contacts;
using KeyPilot.Api.Endpoints.Dev;
using KeyPilot.Api.Endpoints.Documents;
using KeyPilot.Api.Endpoints.Properties;
using KeyPilot.Api.Endpoints.Tasks;
using KeyPilot.Api.Extensions;
using KeyPilot.Application;
using KeyPilot.Infrastructure;
using KeyPilot.Infrastructure.Workflow;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<WorkspaceWorkflowOutboxDispatcherService>();

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    SeedEndpoint.Map(app);
}

app.MapHealthChecks("/health");

WorkspaceConfigEndpoint.Map(app);

var propertyGroup = app.MapGroup("/api/v1/properties").WithTags("Properties").RequireAuthorization();
CreatePropertyEndpoint.Map(propertyGroup);
GetPropertyEndpoint.Map(propertyGroup);
UpdatePropertyEndpoint.Map(propertyGroup);
SubmitOfferEndpoint.Map(propertyGroup);
GoUnconditionalEndpoint.Map(propertyGroup);
SettlePropertyEndpoint.Map(propertyGroup);
CancelPropertyEndpoint.Map(propertyGroup);
ArchivePropertyEndpoint.Map(propertyGroup);

var taskGroup = app.MapGroup("/api/v1/tasks").WithTags("Tasks").RequireAuthorization();
CompleteTaskEndpoint.Map(taskGroup);

var conditionGroup = app.MapGroup("/api/v1/conditions").WithTags("Conditions").RequireAuthorization();
SatisfyConditionEndpoint.Map(conditionGroup);
CompleteConditionEndpoint.Map(conditionGroup);
WaiveConditionEndpoint.Map(conditionGroup);
FailConditionEndpoint.Map(conditionGroup);

DocumentEndpoints.Map(app);
ContactEndpoints.Map(app);

app.Run();

public partial class Program;
