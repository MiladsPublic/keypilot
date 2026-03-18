using FluentValidation;
using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.CreateProperty;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.SubmitOffer;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Properties;

public static class SubmitOfferEndpoint
{
    public sealed record SubmitOfferRequest(
        DateOnly AcceptedOfferDate,
        DateOnly SettlementDate,
        IReadOnlyCollection<CreatePropertyConditionInput>? Conditions);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/submit-offer", HandleAsync)
            .WithName("SubmitOffer")
            .WithSummary("Add accepted offer details to a discovery-stage workspace.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }

    private static async Task<Results<Ok<PropertyDto>, NotFound, ValidationProblem, UnauthorizedHttpResult>> HandleAsync(
        Guid id,
        SubmitOfferRequest request,
        ClaimsPrincipal user,
        IValidator<SubmitOfferCommand> validator,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var command = new SubmitOfferCommand(
            id,
            ownerUserId,
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.Conditions);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await sender.Send(command, cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
