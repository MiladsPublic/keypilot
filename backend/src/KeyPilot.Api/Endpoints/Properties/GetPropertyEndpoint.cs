using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.GetProperties;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Properties;

public static class GetPropertyEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", HandleListAsync)
            .WithName("GetProperties")
            .WithSummary("Get current user's properties.")
            .Produces<IReadOnlyCollection<PropertyDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", HandleAsync)
            .WithName("GetPropertyById")
            .WithSummary("Get a property by id.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<PropertyDto>>, UnauthorizedHttpResult>> HandleListAsync(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var properties = await sender.Send(new GetPropertiesQuery(ownerUserId), cancellationToken);
        return TypedResults.Ok(properties);
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

        var property = await sender.Send(new GetPropertyByIdQuery(id, ownerUserId), cancellationToken);

        return property is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(property);
    }
}
