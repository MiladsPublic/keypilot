using KeyPilot.Application.Conditions.CompleteCondition;
using KeyPilot.Application.Properties.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Conditions;

public static class CompleteConditionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/complete", HandleAsync)
            .WithName("CompleteCondition")
            .WithSummary("Mark a condition complete.")
            .Produces<ConditionDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<ConditionDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CompleteConditionCommand(id), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
