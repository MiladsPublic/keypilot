using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Properties;

public static class GetPropertyEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync)
            .WithName("GetPropertyById")
            .WithSummary("Get a property by id.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<PropertyDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var property = await sender.Send(new GetPropertyByIdQuery(id), cancellationToken);

        return property is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(property);
    }
}
