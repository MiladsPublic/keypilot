using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Common;

public sealed record DocumentDto(
    Guid Id,
    string StorageKey,
    string FileName,
    string Category,
    DateTime CreatedAtUtc)
{
    public static DocumentDto FromDocument(Document document)
    {
        return new DocumentDto(
            document.Id,
            document.StorageKey,
            document.FileName,
            document.Category,
            document.CreatedAtUtc);
    }
}
