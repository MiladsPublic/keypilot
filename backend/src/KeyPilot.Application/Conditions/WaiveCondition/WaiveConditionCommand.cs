using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Conditions.WaiveCondition;

public sealed record WaiveConditionCommand(Guid Id, string OwnerUserId) : IRequest<ConditionDto?>;
