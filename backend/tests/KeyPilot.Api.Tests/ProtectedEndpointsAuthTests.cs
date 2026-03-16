using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KeyPilot.Api.Tests;

public sealed class ProtectedEndpointsAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProtectedEndpointsAuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/v1/properties")]
    [InlineData("/api/v1/properties/00000000-0000-0000-0000-000000000001")]
    [InlineData("/api/v1/tasks/00000000-0000-0000-0000-000000000001/complete")]
    [InlineData("/api/v1/conditions/00000000-0000-0000-0000-000000000001/satisfy")]
    [InlineData("/api/v1/conditions/00000000-0000-0000-0000-000000000001/complete")]
    [InlineData("/api/v1/conditions/00000000-0000-0000-0000-000000000001/waive")]
    [InlineData("/api/v1/conditions/00000000-0000-0000-0000-000000000001/fail")]
    [InlineData("/api/v1/properties/00000000-0000-0000-0000-000000000001/settle")]
    [InlineData("/api/v1/properties/00000000-0000-0000-0000-000000000001/cancel")]
    public async Task ProtectedEndpoints_WithoutToken_ReturnUnauthorized(string path)
    {
        using var client = _factory.CreateClient();

        var response = IsGet(path)
            ? await client.GetAsync(path)
            : await client.PatchAsync(path, content: null);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SatisfyAndLegacyCompleteAlias_BothRequireAuthorization()
    {
        using var client = _factory.CreateClient();
        const string id = "00000000-0000-0000-0000-000000000001";

        var satisfyResponse = await client.PatchAsync($"/api/v1/conditions/{id}/satisfy", content: null);
        var completeResponse = await client.PatchAsync($"/api/v1/conditions/{id}/complete", content: null);

        satisfyResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        completeResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private static bool IsGet(string path)
    {
        return path == "/api/v1/properties" ||
               path == "/api/v1/properties/00000000-0000-0000-0000-000000000001";
    }
}
