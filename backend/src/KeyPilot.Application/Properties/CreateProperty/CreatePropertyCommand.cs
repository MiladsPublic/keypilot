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
    string BuyingMethod,
    decimal? PurchasePrice,
    decimal? DepositAmount,
    string? MethodReference,
    IReadOnlyCollection<CreatePropertyConditionInput>? Conditions,
    string OwnerUserId) : IRequest<CreatePropertyResponse>;
