using FluentAssertions;
using KeyPilot.Application.Properties.TaskGeneration;
using KeyPilot.Domain.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace KeyPilot.Api.Tests;

public sealed class TaskTemplateServiceTests
{
    private readonly ITaskTemplateService _sut = new TaskTemplateServiceAccessor();

    [Theory]
    [InlineData(BuyingMethod.PrivateSale)]
    [InlineData(BuyingMethod.Negotiation)]
    public void GetAcceptedOfferTasks_StandardMethods_ContainsReviewConditions(BuyingMethod method)
    {
        var tasks = _sut.GetAcceptedOfferTasks(method);

        tasks.Should().Contain("Review active conditions");
        tasks.Should().Contain("Confirm lawyer details");
        tasks.Should().Contain("Confirm settlement date");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Auction_ContainsAuctionSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Auction);

        tasks.Should().Contain("Confirm auction registration");
        tasks.Should().Contain("Review auction terms");
        tasks.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Tender_ContainsTenderSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Tender);

        tasks.Should().Contain("Prepare tender submission");
        tasks.Should().Contain("Confirm tender deadline");
        tasks.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Deadline_ContainsDeadlineSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Deadline);

        tasks.Should().Contain("Review deadline sale terms");
        tasks.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_AllMethods_AlwaysContainLawyerAndSettlement()
    {
        var methods = new[] { BuyingMethod.PrivateSale, BuyingMethod.Auction, BuyingMethod.Negotiation, BuyingMethod.Tender, BuyingMethod.Deadline };

        foreach (var method in methods)
        {
            var tasks = _sut.GetAcceptedOfferTasks(method);
            tasks.Should().Contain("Confirm lawyer details", $"method {method} should include lawyer task");
            tasks.Should().Contain("Confirm settlement date", $"method {method} should include settlement task");
        }
    }

    /// <summary>
    /// Wraps the internal TaskTemplateService so it can be instantiated in tests.
    /// </summary>
    private sealed class TaskTemplateServiceAccessor : ITaskTemplateService
    {
        // Resolve the internal implementation via DI to test the real class.
        private readonly ITaskTemplateService _inner;

        public TaskTemplateServiceAccessor()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            KeyPilot.Application.DependencyInjection.AddApplication(services);
            var provider = services.BuildServiceProvider();
            _inner = provider.GetRequiredService<ITaskTemplateService>();
        }

        public IReadOnlyCollection<string> GetAcceptedOfferTasks(BuyingMethod buyingMethod) => _inner.GetAcceptedOfferTasks(buyingMethod);
        public IReadOnlyCollection<string> GetConditionTasks(ConditionType conditionType) => _inner.GetConditionTasks(conditionType);
        public IReadOnlyCollection<string> GetPreSettlementTasks() => _inner.GetPreSettlementTasks();
        public IReadOnlyCollection<string> GetSettlementTasks() => _inner.GetSettlementTasks();
    }
}
