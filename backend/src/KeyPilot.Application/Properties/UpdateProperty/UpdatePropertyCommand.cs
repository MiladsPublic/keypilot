using KeyPilot.Application.Properties.GetPropertyById;
using MediatR;

namespace KeyPilot.Application.Properties.UpdateProperty;

public sealed record UpdatePropertyCommand(
    Guid Id,
    string OwnerUserId,
    string? Address,
    DateOnly? AcceptedOfferDate,
    DateOnly? SettlementDate,
    decimal? PurchasePrice,
    decimal? DepositAmount,
    string? MethodReference) : IRequest<PropertyDto?>;
