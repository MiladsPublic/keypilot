using KeyPilot.Application.Properties.CreateProperty;
using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.SubmitOffer;

public sealed record SubmitOfferCommand(
    Guid Id,
    string OwnerUserId,
    DateOnly AcceptedOfferDate,
    DateOnly SettlementDate,
    IReadOnlyCollection<CreatePropertyConditionInput>? Conditions) : IRequest<PropertyDto?>;
