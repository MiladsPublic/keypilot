using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Tasks.CompleteTask;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KeyPilot.Api.Endpoints.Tasks;

public static class CompleteTaskEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/complete", HandleAsync)
            .WithName("CompleteTask")
            .WithSummary("Mark a task complete.")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CompleteTaskCommand(id), cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
