using KeyPilot.Api.Auth;
using KeyPilot.Application.Contacts.AddContact;
using KeyPilot.Application.Contacts.DeleteContact;
using KeyPilot.Application.Properties.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Contacts;

public static class ContactEndpoints
{
    public sealed record AddContactRequest(string Role, string Name, string? Email, string? Phone);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/properties/{propertyId:guid}/contacts")
            .WithTags("Contacts")
            .RequireAuthorization();

        group.MapPost("/", HandleAddAsync)
            .WithName("AddContact")
            .WithSummary("Add a contact to a property.")
            .Produces<ContactDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        var byId = app.MapGroup("/api/v1/contacts")
            .WithTags("Contacts")
            .RequireAuthorization();

        byId.MapDelete("/{id:guid}", HandleDeleteAsync)
            .WithName("DeleteContact")
            .WithSummary("Delete a contact.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Created<ContactDto>, NotFound, UnauthorizedHttpResult>> HandleAddAsync(
        Guid propertyId,
        AddContactRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(
            new AddContactCommand(propertyId, request.Role, request.Name, request.Email, request.Phone, ownerUserId),
            cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Created($"/api/v1/contacts/{result.Id}", result);
    }

    private static async Task<Results<NoContent, NotFound, UnauthorizedHttpResult>> HandleDeleteAsync(
        Guid id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var ownerUserId = user.GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return TypedResults.Unauthorized();
        }

        var deleted = await sender.Send(new DeleteContactCommand(id, ownerUserId), cancellationToken);

        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
