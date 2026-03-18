using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.TaskGeneration;

public record TaskTemplate(string Title, string Description);

public interface ITaskTemplateService
{
    IReadOnlyCollection<TaskTemplate> GetDiscoveryTasks(BuyingMethod buyingMethod);

    IReadOnlyCollection<TaskTemplate> GetAcceptedOfferTasks(BuyingMethod buyingMethod);

    IReadOnlyCollection<TaskTemplate> GetConditionTasks(ConditionType conditionType);

    IReadOnlyCollection<TaskTemplate> GetPreSettlementTasks();

    IReadOnlyCollection<TaskTemplate> GetSettlementTasks();
}
