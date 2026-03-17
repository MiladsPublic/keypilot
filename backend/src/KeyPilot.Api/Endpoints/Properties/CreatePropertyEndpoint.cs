using FluentValidation;
using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.CreateProperty;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Properties;

public static class CreatePropertyEndpoint
{
    public sealed record CreatePropertyRequest(
        string Address,
        DateOnly AcceptedOfferDate,
        DateOnly SettlementDate,
        string? BuyingMethod,
        decimal? PurchasePrice,
        decimal? DepositAmount,
        IReadOnlyCollection<CreatePropertyConditionInput>? Conditions);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateProperty")
            .WithSummary("Create a property workspace entry.")
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<Results<Created<CreatePropertyResponse>, ValidationProblem, UnauthorizedHttpResult>> HandleAsync(
        CreatePropertyRequest request,
        ClaimsPrincipal user,
        IValidator<CreatePropertyCommand> validator,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var command = new CreatePropertyCommand(
            request.Address,
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.BuyingMethod ?? "private_sale",
            request.PurchasePrice,
            request.DepositAmount,
            request.Conditions,
            ownerUserId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/api/v1/properties/{response.Id}", response);
    }
}
