using MediatR;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed record CreatePropertyConditionInput(
    string Type,
    int? DaysFromAcceptedOffer,
    DateOnly? DueDate);

public sealed record CreatePropertyCommand(
    string Address,
    DateOnly AcceptedOfferDate,
    DateOnly SettlementDate,
    decimal? PurchasePrice,
    decimal? DepositAmount,
    IReadOnlyCollection<CreatePropertyConditionInput>? Conditions,
    string OwnerUserId) : IRequest<CreatePropertyResponse>;
