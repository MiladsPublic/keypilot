using FluentValidation;
using KeyPilot.Application.Properties.CreateProperty;

namespace KeyPilot.Application.Properties.SubmitOffer;

public sealed class SubmitOfferValidator : AbstractValidator<SubmitOfferCommand>
{
    private static readonly HashSet<string> ValidConditionTypes =
        ["finance", "building_report", "lim", "insurance", "solicitor_approval"];

    public SubmitOfferValidator()
    {
        RuleFor(x => x.AcceptedOfferDate).NotEmpty();
        RuleFor(x => x.SettlementDate).NotEmpty();

        RuleFor(x => x.SettlementDate)
            .GreaterThanOrEqualTo(x => x.AcceptedOfferDate)
            .WithMessage("Settlement date must be on or after the accepted offer date.");

        RuleForEach(x => x.Conditions)
            .ChildRules(condition =>
            {
                condition.RuleFor(c => c.Type)
                    .Must(type => ValidConditionTypes.Contains(type.Trim().ToLowerInvariant()))
                    .WithMessage("Invalid condition type.");
            })
            .When(x => x.Conditions is { Count: > 0 });
    }
}
