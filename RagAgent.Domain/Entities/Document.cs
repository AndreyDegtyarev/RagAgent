using RagAgent.Domain.Enums;
using RagAgent.Domain.Interfaces;

namespace RagAgent.Domain.Entities;

public class Document : IDomainEntity
{
    private Document()
    {
    }


    public Document(string name, string contentType)
    {
        Id = Guid.CreateVersion7();
        OriginalFileName = name;
        ContentType = contentType;
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

    public uint RowVersion { get; private set; } = default!;


    public void SetStoragePath(string storagePath)
    {
        StoragePath = storagePath;
    }
    
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


    public void MarkCompleted()
    {
        if (Status != DocumentStatus.Processing)
            throw new InvalidOperationException();


        Status = DocumentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }


    public void MarkFailed()
    {
        Status = DocumentStatus.Failed;
    }
}