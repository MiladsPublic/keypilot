using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.CancelProperty;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Properties;

public static class CancelPropertyEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/cancel", HandleAsync)
            .WithName("CancelProperty")
            .WithSummary("Mark a property as cancelled.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<PropertyDto>, NotFound, UnauthorizedHttpResult>> HandleAsync(
        Guid id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(new CancelPropertyCommand(id, ownerUserId), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
