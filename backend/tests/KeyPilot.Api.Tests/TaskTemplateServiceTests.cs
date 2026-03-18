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

        tasks.Select(t => t.Title).Should().Contain("Review active conditions");
        tasks.Select(t => t.Title).Should().Contain("Confirm lawyer details");
        tasks.Select(t => t.Title).Should().Contain("Confirm settlement date");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Auction_ContainsAuctionSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Auction);
        var titles = tasks.Select(t => t.Title).ToList();

        titles.Should().Contain("Confirm auction registration");
        titles.Should().Contain("Review auction terms");
        titles.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Tender_ContainsTenderSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Tender);
        var titles = tasks.Select(t => t.Title).ToList();

        titles.Should().Contain("Prepare tender submission");
        titles.Should().Contain("Confirm tender deadline");
        titles.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_Deadline_ContainsDeadlineSpecificTasks()
    {
        var tasks = _sut.GetAcceptedOfferTasks(BuyingMethod.Deadline);
        var titles = tasks.Select(t => t.Title).ToList();

        titles.Should().Contain("Review deadline sale terms");
        titles.Should().NotContain("Review active conditions");
    }

    [Fact]
    public void GetAcceptedOfferTasks_AllMethods_AlwaysContainLawyerAndSettlement()
    {
        var methods = new[] { BuyingMethod.PrivateSale, BuyingMethod.Auction, BuyingMethod.Negotiation, BuyingMethod.Tender, BuyingMethod.Deadline };

        foreach (var method in methods)
        {
            var titles = _sut.GetAcceptedOfferTasks(method).Select(t => t.Title).ToList();
            titles.Should().Contain("Confirm lawyer details", $"method {method} should include lawyer task");
            titles.Should().Contain("Confirm settlement date", $"method {method} should include settlement task");
        }
    }

    [Fact]
    public void AllTasks_HaveDescriptions()
    {
        var methods = new[] { BuyingMethod.PrivateSale, BuyingMethod.Auction, BuyingMethod.Negotiation, BuyingMethod.Tender, BuyingMethod.Deadline };

        foreach (var method in methods)
        {
            foreach (var template in _sut.GetDiscoveryTasks(method))
            {
                template.Description.Should().NotBeNullOrWhiteSpace($"discovery task '{template.Title}' for {method}");
            }

            foreach (var template in _sut.GetAcceptedOfferTasks(method))
            {
                template.Description.Should().NotBeNullOrWhiteSpace($"accepted offer task '{template.Title}' for {method}");
            }
        }

        foreach (var template in _sut.GetPreSettlementTasks())
        {
            template.Description.Should().NotBeNullOrWhiteSpace($"pre-settlement task '{template.Title}'");
        }

        foreach (var template in _sut.GetSettlementTasks())
        {
            template.Description.Should().NotBeNullOrWhiteSpace($"settlement task '{template.Title}'");
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

        public IReadOnlyCollection<TaskTemplate> GetDiscoveryTasks(BuyingMethod buyingMethod) => _inner.GetDiscoveryTasks(buyingMethod);
        public IReadOnlyCollection<TaskTemplate> GetAcceptedOfferTasks(BuyingMethod buyingMethod) => _inner.GetAcceptedOfferTasks(buyingMethod);
        public IReadOnlyCollection<TaskTemplate> GetConditionTasks(ConditionType conditionType) => _inner.GetConditionTasks(conditionType);
        public IReadOnlyCollection<TaskTemplate> GetPreSettlementTasks() => _inner.GetPreSettlementTasks();
        public IReadOnlyCollection<TaskTemplate> GetSettlementTasks() => _inner.GetSettlementTasks();
    }
}
