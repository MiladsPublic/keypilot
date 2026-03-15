using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Conditions.CompleteCondition;

public sealed record CompleteConditionCommand(Guid Id, string OwnerUserId) : IRequest<ConditionDto?>;
