using MediatR;

namespace KeyPilot.Application.Properties.Reminders.ProcessWorkspaceReminderTick;

public sealed record ProcessWorkspaceReminderTickCommand(
    Guid WorkspaceId,
    DateTime TriggeredAtUtc) : IRequest<bool>;