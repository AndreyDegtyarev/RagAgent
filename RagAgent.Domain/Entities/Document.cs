using RagAgent.Domain.Enums;
using RagAgent.Domain.Interfaces;

namespace RagAgent.Domain.Entities;

public class Document : IDomainEntity
{
    private Document()
    {
    }


    public Document(string name)
    {
        Id = Guid.CreateVersion7();
        OriginalFileName = name;
        Status = DocumentStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }


    public Guid Id { get; private set; }

    public string OriginalFileName { get; private set; } = default!;

    public string StoragePath { get; private set; } = default!;

    public string ContentType { get; private set; } = default!;

    public long FileSize { get; private set; }

    public string? FileHash { get; private set; }

    public DocumentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = default!;


    public void MarkFileStored()
    {
        if (Status != DocumentStatus.Created)
            throw new InvalidOperationException();


        Status = DocumentStatus.FileStored;
    }


    public void StartProcessing()
    {
        if (Status != DocumentStatus.FileStored)
            throw new InvalidOperationException();


        Status = DocumentStatus.Processing;
    }
}