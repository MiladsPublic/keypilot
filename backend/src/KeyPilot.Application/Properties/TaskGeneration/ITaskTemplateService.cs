using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

public interface ITaskTemplateService
{
    IReadOnlyCollection<string> GetAcceptedOfferTasks(BuyingMethod buyingMethod);

    IReadOnlyCollection<string> GetConditionTasks(ConditionType conditionType);

    IReadOnlyCollection<string> GetPreSettlementTasks();

    IReadOnlyCollection<string> GetSettlementTasks();
}
