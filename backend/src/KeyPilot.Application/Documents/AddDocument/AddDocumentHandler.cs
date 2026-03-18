using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Documents.AddDocument;

public sealed class AddDocumentHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<AddDocumentCommand, DocumentDto?>
{
    public async Task<DocumentDto?> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.PropertyId, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var document = property.AddDocument(
            request.StorageKey,
            request.FileName,
            request.Category,
            dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);

        return DocumentDto.FromDocument(document);
    }
}
