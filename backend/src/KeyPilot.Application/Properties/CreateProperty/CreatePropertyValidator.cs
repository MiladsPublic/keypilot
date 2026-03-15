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

    public CreatePropertyValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.AcceptedOfferDate)
            .NotEmpty();

        RuleFor(x => x.SettlementDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.AcceptedOfferDate);

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
