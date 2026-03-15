using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.SettleProperty;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Properties;

public static class SettlePropertyEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/settle", HandleAsync)
            .WithName("SettleProperty")
            .WithSummary("Mark a property as settled.")
            .Produces<PropertyDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<PropertyDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SettlePropertyCommand(id), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
