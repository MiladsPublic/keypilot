using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class Document : AuditableEntity
{
    private Document()
    {
    }

    internal Document(
        Guid propertyId,
        string storageKey,
        string fileName,
        string category,
        DateTime createdAtUtc)
    {
        PropertyId = propertyId;
        StorageKey = storageKey;
        FileName = fileName;
        Category = category;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PropertyId { get; private set; }

    public string StorageKey { get; private set; } = string.Empty;

    public string FileName { get; private set; } = string.Empty;

    public string Category { get; private set; } = string.Empty;
}
