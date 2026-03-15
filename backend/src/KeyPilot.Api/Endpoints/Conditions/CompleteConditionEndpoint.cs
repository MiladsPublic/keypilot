using KeyPilot.Api.Auth;
using KeyPilot.Application.Conditions.CompleteCondition;
using KeyPilot.Application.Properties.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Conditions;

public static class CompleteConditionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/complete", HandleAsync)
            .WithName("CompleteCondition")
            .WithSummary("Mark a condition complete.")
            .Produces<ConditionDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<ConditionDto>, NotFound, UnauthorizedHttpResult>> HandleAsync(
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

        var result = await sender.Send(new CompleteConditionCommand(id, ownerUserId), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
