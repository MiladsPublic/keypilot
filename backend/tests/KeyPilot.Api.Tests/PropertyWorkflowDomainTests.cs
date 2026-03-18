using FluentAssertions;
using KeyPilot.Domain.Properties;

namespace KeyPilot.Api.Tests;

public sealed class PropertyWorkflowDomainTests
{
    [Fact]
    public void RecalculateStatus_StaysConditional_WhenAllConditionsClosed_WithoutExplicitConfirmation()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "1 Test Street",
            today,
            today.AddDays(30),
            1000000m,
            100000m,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        var condition = property.AddCondition(ConditionType.Finance, today.AddDays(5), createdAt);
        condition.MarkSatisfied(createdAt.AddHours(1));

        property.RecalculateStatus(today);

        property.Status.Should().Be(PropertyStatus.Conditional);
        property.UnconditionalDate.Should().BeNull();
    }

    [Fact]
    public void GoUnconditional_SetsUnconditionalStatusAndDate()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "1 Test Street",
            today,
            today.AddDays(30),
            1000000m,
            100000m,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        var condition = property.AddCondition(ConditionType.Finance, today.AddDays(5), createdAt);
        condition.MarkSatisfied(createdAt.AddHours(1));

        property.GoUnconditional(today);

        property.Status.Should().Be(PropertyStatus.Unconditional);
        property.UnconditionalDate.Should().Be(today);
    }

    [Fact]
    public void RecalculateStatus_KeepsOriginalUnconditionalDate_AfterSubsequentRecalculations()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var dayOne = DateOnly.FromDateTime(createdAt);
        var dayTwo = dayOne.AddDays(1);
        var property = Property.Create(
            "2 Test Street",
            dayOne,
            dayOne.AddDays(20),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        var condition = property.AddCondition(ConditionType.Lim, dayOne.AddDays(3), createdAt);
        condition.MarkSatisfied(createdAt.AddHours(1));

        property.GoUnconditional(dayOne);
        property.RecalculateStatus(dayTwo);

        property.Status.Should().Be(PropertyStatus.Unconditional);
        property.UnconditionalDate.Should().Be(dayOne);
    }

    [Fact]
    public void RecalculateStatus_TransitionsToSettlementPending_WhenUnconditionalAndSettlementWithin14Days()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "7 Test Street",
            today,
            today.AddDays(10),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        property.GoUnconditional(today);
        property.RecalculateStatus(today);

        property.Status.Should().Be(PropertyStatus.SettlementPending);
    }

    [Fact]
    public void RecalculateStatus_StaysConditional_WhenAnyConditionFailed()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "3 Test Street",
            today,
            today.AddDays(14),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        var condition = property.AddCondition(ConditionType.BuildingReport, today.AddDays(2), createdAt);
        condition.MarkFailed();

        property.RecalculateStatus(today);

        property.Status.Should().Be(PropertyStatus.Conditional);
        property.UnconditionalDate.Should().BeNull();
    }

    [Fact]
    public void RecalculateStatus_MarksOverduePendingConditionExpired()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "4 Test Street",
            today,
            today.AddDays(14),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        var condition = property.AddCondition(ConditionType.Insurance, today.AddDays(-1), createdAt);

        property.RecalculateStatus(today);

        condition.Status.Should().Be(ConditionStatus.Expired);
        property.Status.Should().Be(PropertyStatus.Conditional);
    }

    [Fact]
    public void MarkSettlementComplete_SetsSettledDateAndSettledStatus()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var property = Property.Create(
            "5 Test Street",
            DateOnly.FromDateTime(createdAt),
            DateOnly.FromDateTime(createdAt).AddDays(10),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        property.MarkSettlementComplete(createdAt.AddDays(11));

        property.Status.Should().Be(PropertyStatus.Settled);
        property.SettledDate.Should().Be(DateOnly.FromDateTime(createdAt.AddDays(11)));
    }

    [Fact]
    public void MarkCancelled_SetsCancelledAndPreventsFurtherStageChanges()
    {
        var createdAt = new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(createdAt);
        var property = Property.Create(
            "6 Test Street",
            today,
            today.AddDays(21),
            null,
            null,
            "user-1",
            workspaceId: null,
            createdAtUtc: createdAt);

        property.MarkCancelled(createdAt.AddDays(2));
        property.RecalculateStatus(today.AddDays(3));

        property.Status.Should().Be(PropertyStatus.Cancelled);
        property.CancelledDate.Should().Be(today.AddDays(2));
    }
}
