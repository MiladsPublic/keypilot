using FluentValidation;

namespace KeyPilot.Application.Properties.CreateProperty;

public sealed class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
{
    private static readonly string[] SupportedConditions =
    [
        "finance",
        "building_report",
        "lim",
        "insurance",
        "solicitor_approval"
    ];

    private static readonly string[] SupportedBuyingMethods =
    [
        "private_sale",
        "auction",
        "negotiation",
        "tender",
        "deadline"
    ];

    public CreatePropertyValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.OwnerUserId)
            .NotEmpty();

        RuleFor(x => x.SettlementDate)
            .GreaterThanOrEqualTo(x => x.AcceptedOfferDate!.Value)
            .When(x => x.AcceptedOfferDate.HasValue && x.SettlementDate.HasValue);

        RuleFor(x => x.BuyingMethod)
            .NotEmpty()
            .Must(method => SupportedBuyingMethods.Contains(method.Trim().ToLowerInvariant()))
            .WithMessage("Unsupported buying method.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThan(0)
            .When(x => x.PurchasePrice.HasValue);

        RuleFor(x => x.DepositAmount)
            .GreaterThan(0)
            .When(x => x.DepositAmount.HasValue);

        RuleForEach(x => x.Conditions)
            .ChildRules(condition =>
            {
                condition.RuleFor(item => item.Type)
                    .NotEmpty()
                    .Must(type => SupportedConditions.Contains(type.Trim().ToLowerInvariant()))
                    .WithMessage("Unsupported condition type.");

                condition.RuleFor(item => item)
                    .Must(item => item.DueDate.HasValue || item.DaysFromAcceptedOffer.HasValue)
                    .WithMessage("Condition requires either dueDate or daysFromAcceptedOffer.");

                condition.RuleFor(item => item.DaysFromAcceptedOffer)
                    .GreaterThanOrEqualTo(0)
                    .When(item => item.DaysFromAcceptedOffer.HasValue);
            });
    }
}
