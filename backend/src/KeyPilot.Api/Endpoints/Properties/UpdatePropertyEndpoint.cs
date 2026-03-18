using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.UpdateProperty;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Properties;

public static class UpdatePropertyEndpoint
{
    public sealed record UpdatePropertyRequest(
        string? Address,
        DateOnly? AcceptedOfferDate,
        DateOnly? SettlementDate,
        decimal? PurchasePrice,
        decimal? DepositAmount,
        string? MethodReference);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}", HandleAsync)
            .WithName("UpdateProperty")
            .WithSummary("Update property details.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<PropertyDto>, NotFound, UnauthorizedHttpResult>> HandleAsync(
        Guid id,
        UpdatePropertyRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(new UpdatePropertyCommand(
            id,
            ownerUserId,
            request.Address,
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.PurchasePrice,
            request.DepositAmount,
            request.MethodReference), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
