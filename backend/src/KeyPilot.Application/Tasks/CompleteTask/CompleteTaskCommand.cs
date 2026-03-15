using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Tasks.CompleteTask;

public sealed record CompleteTaskCommand(Guid Id) : IRequest<TaskDto?>;
