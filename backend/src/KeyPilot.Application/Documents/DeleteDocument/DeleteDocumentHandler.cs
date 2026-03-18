using KeyPilot.Application.Abstractions.Persistence;
using MediatR;

namespace KeyPilot.Application.Documents.DeleteDocument;

public sealed class DeleteDocumentHandler(
    IApplicationDbContext dbContext) : IRequestHandler<DeleteDocumentCommand, bool>
{
    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await dbContext.GetDocumentByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        dbContext.RemoveDocument(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
