using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Conditions.FailCondition;

public sealed record FailConditionCommand(Guid Id, string OwnerUserId) : IRequest<ConditionDto?>;
