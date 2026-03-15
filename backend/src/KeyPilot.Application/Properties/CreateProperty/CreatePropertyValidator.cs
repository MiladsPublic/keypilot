using FluentValidation;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .When(x => x.PurchasePrice.HasValue);

        RuleFor(x => x)
            .Must(command => !command.OfferAcceptedDate.HasValue ||
                             !command.SettlementDate.HasValue ||
                             command.SettlementDate >= command.OfferAcceptedDate)
            .WithMessage("Settlement date must be on or after the offer accepted date.");
    }
}
