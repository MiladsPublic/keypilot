using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.CreateProperty;
using KeyPilot.Domain.Properties;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Dev;

public static class SeedEndpoint
{
    public sealed record SeedRequest(string? OwnerUserId, string? Address);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/dev/seed", HandleAsync)
            .WithName("SeedDevData")
            .WithSummary("Seed a sample purchase for local development.")
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created);
    }

    private static async Task<Created<CreatePropertyResponse>> HandleAsync(
        SeedRequest? request,
        IApplicationDbContext dbContext,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    {
        var nowUtc = dateTimeProvider.UtcNow;
        var today = DateOnly.FromDateTime(nowUtc);
        var acceptedOfferDate = today.AddDays(-2);
        var settlementDate = today.AddDays(21);
        var financeDue = today.AddDays(5);
        var buildingDue = today.AddDays(6);
        var limDue = today.AddDays(10);

        var property = Property.Create(
            request?.Address?.Trim() is { Length: > 0 } address
                ? address
                : "12 Beach Road, Takapuna",
            acceptedOfferDate,
            settlementDate,
            1250000m,
            250000m,
            request?.OwnerUserId?.Trim() is { Length: > 0 } ownerUserId
                ? ownerUserId
                : "dev-seed-user",
            workspaceId: null,
            createdAtUtc: nowUtc);

        var finance = property.AddCondition(ConditionType.Finance, financeDue, nowUtc);
        var building = property.AddCondition(ConditionType.BuildingReport, buildingDue, nowUtc);
        var lim = property.AddCondition(ConditionType.Lim, limDue, nowUtc);

        property.AddTask("Confirm lawyer details", TaskStage.AcceptedOffer, today.AddDays(1), null, nowUtc);
        property.AddTask("Review active conditions", TaskStage.AcceptedOffer, today.AddDays(2), null, nowUtc);

        property.AddTask("Submit finance documents", TaskStage.Conditional, financeDue.AddDays(-2), finance.Id, nowUtc);
        property.AddTask("Confirm lender approval", TaskStage.Conditional, financeDue.AddDays(-1), finance.Id, nowUtc);
        property.AddTask("Book building inspection", TaskStage.Conditional, buildingDue.AddDays(-3), building.Id, nowUtc);
        property.AddTask("Review building report", TaskStage.Conditional, buildingDue.AddDays(-1), building.Id, nowUtc);
        property.AddTask("Obtain LIM report", TaskStage.Conditional, limDue.AddDays(-3), lim.Id, nowUtc);
        property.AddTask("Review LIM findings", TaskStage.Conditional, limDue.AddDays(-1), lim.Id, nowUtc);

        property.AddTask("Confirm insurance active from settlement date", TaskStage.PreSettlement, settlementDate.AddDays(-5), null, nowUtc);
        property.AddTask("Complete final inspection", TaskStage.PreSettlement, settlementDate.AddDays(-3), null, nowUtc);
        property.AddTask("Confirm settlement funds ready", TaskStage.PreSettlement, settlementDate.AddDays(-1), null, nowUtc);
        property.AddTask("Keys collected", TaskStage.Settlement, settlementDate, null, nowUtc);

        property.RecalculateStatus();

        await dbContext.AddPropertyAsync(property, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = CreatePropertyResponse.FromProperty(property);
        return TypedResults.Created($"/api/v1/properties/{response.Id}", response);
    }
}
