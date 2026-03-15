using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Tasks.CompleteTask;

public sealed class CompleteTaskHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<CompleteTaskCommand, TaskDto?>
{
    public async Task<TaskDto?> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await dbContext.GetTaskByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            return null;
        }

        task.MarkCompleted(dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TaskDto.FromTask(task);
    }
}
