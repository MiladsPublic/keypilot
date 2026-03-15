using MediatR;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed record CreatePropertyCommand(
    string Address,
    DateOnly? OfferAcceptedDate,
    DateOnly? SettlementDate,
    decimal? PurchasePrice) : IRequest<CreatePropertyResponse>;
