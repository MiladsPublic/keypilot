using KeyPilot.Api.Auth;
using KeyPilot.Application.Documents.AddDocument;
using KeyPilot.Application.Documents.DeleteDocument;
using KeyPilot.Application.Properties.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace KeyPilot.Api.Endpoints.Documents;

public static class DocumentEndpoints
{
    public sealed record AddDocumentRequest(string StorageKey, string FileName, string Category);

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/properties/{propertyId:guid}/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        group.MapPost("/", HandleAddAsync)
            .WithName("AddDocument")
            .WithSummary("Add a document to a property.")
            .Produces<DocumentDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);

        var byId = app.MapGroup("/api/v1/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        byId.MapDelete("/{id:guid}", HandleDeleteAsync)
            .WithName("DeleteDocument")
            .WithSummary("Delete a document.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Created<DocumentDto>, NotFound, UnauthorizedHttpResult>> HandleAddAsync(
        Guid propertyId,
        AddDocumentRequest request,
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
            new AddDocumentCommand(propertyId, request.StorageKey, request.FileName, request.Category, ownerUserId),
            cancellationToken);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Created($"/api/v1/documents/{result.Id}", result);
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

        var deleted = await sender.Send(new DeleteDocumentCommand(id, ownerUserId), cancellationToken);

        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
