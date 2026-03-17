using FluentAssertions;
using KeyPilot.Application.Conditions.CompleteCondition;
using KeyPilot.Application.Conditions.FailCondition;
using KeyPilot.Application.Conditions.WaiveCondition;
using KeyPilot.Application.Properties.CancelProperty;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.SettleProperty;
using KeyPilot.Application.Tasks.CompleteTask;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;

namespace KeyPilot.Api.Tests;

public sealed class OwnershipAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OwnershipAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OwnerScopedEndpoints_DifferentAuthenticatedUser_ReturnsNotFound()
    {
        using var factory = CreateFactoryWithTestAuthAndOwnerAwareSender();
        using var ownerClient = CreateAuthenticatedClient(factory, "owner-a");
        using var intruderClient = CreateAuthenticatedClient(factory, "owner-b");

        var checks = new (HttpMethod method, string path)[]
        {
            (HttpMethod.Get, $"/api/v1/properties/{OwnerAwareSender.SeedPropertyId}"),
            (HttpMethod.Patch, $"/api/v1/properties/{OwnerAwareSender.SeedPropertyId}/settle"),
            (HttpMethod.Patch, $"/api/v1/properties/{OwnerAwareSender.SeedPropertyId}/cancel"),
            (HttpMethod.Patch, $"/api/v1/tasks/{OwnerAwareSender.SeedTaskId}/complete"),
            (HttpMethod.Patch, $"/api/v1/conditions/{OwnerAwareSender.SeedConditionId}/satisfy"),
            (HttpMethod.Patch, $"/api/v1/conditions/{OwnerAwareSender.SeedConditionId}/waive"),
            (HttpMethod.Patch, $"/api/v1/conditions/{OwnerAwareSender.SeedConditionId}/fail")
        };

        foreach (var (method, path) in checks)
        {
            using var ownerResponse = await SendAsync(ownerClient, method, path);
            ownerResponse.StatusCode.Should().Be(HttpStatusCode.OK, $"{method} {path} should be visible to the owner");

            using var intruderResponse = await SendAsync(intruderClient, method, path);
            intruderResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, $"{method} {path} should not expose another user's data");
        }
    }

    private WebApplicationFactory<Program> CreateFactoryWithTestAuthAndOwnerAwareSender()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

                services.RemoveAll<ISender>();
                services.AddSingleton<ISender, OwnerAwareSender>();
            });
        });
    }

    private static HttpClient CreateAuthenticatedClient(WebApplicationFactory<Program> factory, string userId)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId);
        return client;
    }

    private static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpMethod method, string path)
    {
        using var request = new HttpRequestMessage(method, path);
        return await client.SendAsync(request);
    }

    private sealed class OwnerAwareSender : ISender
    {
        private const string OwnerId = "owner-a";

        public static readonly Guid SeedPropertyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid SeedTaskId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid SeedConditionId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        private static readonly ConditionDto SeedCondition = new(
            SeedConditionId,
            Type: "finance",
            DueDate: new DateOnly(2026, 3, 23),
            Status: "pending",
            CompletedAtUtc: null);

        private static readonly TaskDto SeedTask = new(
            SeedTaskId,
            ConditionId: SeedConditionId,
            Title: "Upload finance documents",
            Stage: "conditional",
            DueDate: new DateOnly(2026, 3, 20),
            Status: "pending",
            CompletedAtUtc: null);

        private static readonly PropertyDto SeedProperty = new(
            SeedPropertyId,
            WorkspaceId: null,
            Address: "42 Ownership Lane",
            WorkspaceStage: "conditional",
            Status: "conditional",
            AcceptedOfferDate: new DateOnly(2026, 3, 16),
            UnconditionalDate: null,
            SettlementDate: new DateOnly(2026, 4, 30),
            SettledDate: null,
            CancelledDate: null,
            DaysUntilSettlement: 45,
            PurchasePrice: 1500000m,
            DepositAmount: 150000m,
            Reminders: Array.Empty<WorkspaceReminderDto>(),
            Conditions: new[] { SeedCondition },
            Tasks: new[] { SeedTask },
            TaskSummary: new TaskSummaryDto(Completed: 0, Total: 1, Pending: 1),
            ReadinessSummary: new PurchaseReadinessDto(
                Mode: "conditional",
                BlockingConditions: 1,
                OpenConditions: 1,
                OverdueConditions: 0,
                PendingTasks: 1,
                OverdueTasks: 0,
                SettlementTasksRemaining: 0,
                IsReadyToSettle: false,
                NextAction: "Satisfy finance condition"),
            CreatedAtUtc: new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc));

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            object? response = request switch
            {
                GetPropertyByIdQuery query when IsOwnerAndSeedProperty(query.OwnerUserId, query.Id) => SeedProperty,
                SettlePropertyCommand command when IsOwnerAndSeedProperty(command.OwnerUserId, command.Id) => SeedProperty,
                CancelPropertyCommand command when IsOwnerAndSeedProperty(command.OwnerUserId, command.Id) => SeedProperty,
                CompleteTaskCommand command when command.OwnerUserId == OwnerId && command.Id == SeedTaskId => SeedTask,
                CompleteConditionCommand command when command.OwnerUserId == OwnerId && command.Id == SeedConditionId => SeedCondition,
                WaiveConditionCommand command when command.OwnerUserId == OwnerId && command.Id == SeedConditionId => SeedCondition,
                FailConditionCommand command when command.OwnerUserId == OwnerId && command.Id == SeedConditionId => SeedCondition,
                _ => null
            };

            return Task.FromResult((TResponse?)response)!;
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException($"Unexpected non-generic MediatR request type: {request.GetType().Name}");
        }

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            throw new NotSupportedException($"Unexpected MediatR command without response: {request.GetType().Name}");
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return EmptyStream<TResponse>();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            return EmptyStream<object?>();
        }

        private static bool IsOwnerAndSeedProperty(string ownerUserId, Guid propertyId)
        {
            return ownerUserId == OwnerId && propertyId == SeedPropertyId;
        }

        private static async IAsyncEnumerable<T> EmptyStream<T>()
        {
            yield break;
        }
    }
}
