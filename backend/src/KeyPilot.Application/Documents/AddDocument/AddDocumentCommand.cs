using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Documents.AddDocument;

public sealed record AddDocumentCommand(
    Guid PropertyId,
    string StorageKey,
    string FileName,
    string Category,
    string OwnerUserId) : IRequest<DocumentDto?>;
