using KeyPilot.Application.Properties.Common;
using MediatR;

namespace KeyPilot.Application.Documents.DeleteDocument;

public sealed record DeleteDocumentCommand(Guid Id, string OwnerUserId) : IRequest<bool>;
