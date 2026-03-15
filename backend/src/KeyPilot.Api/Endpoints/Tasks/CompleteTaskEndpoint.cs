using KeyPilot.Api.Auth;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Tasks.CompleteTask;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Tasks;

public static class CompleteTaskEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/complete", HandleAsync)
            .WithName("CompleteTask")
            .WithSummary("Mark a task complete.")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<TaskDto>, NotFound, UnauthorizedHttpResult>> HandleAsync(
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

        var result = await sender.Send(new CompleteTaskCommand(id, ownerUserId), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
