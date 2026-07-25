using RagAgent.Domain.Interfaces;

namespace RagAgent.Domain.Entities;

public class DocumentChunk : IDomainEntity
{
    public Guid Id { get; private set; }

    public Guid DocumentId { get; private set; }

    public int Order { get; private set; }

    public string Text { get; private set; } = default!;

    public int TokenCount { get; private set; }

    public string Hash { get; private set; } = default!;


    private DocumentChunk()
    {
    }


    public DocumentChunk(
        Guid documentId,
        string content,
        string contentHash)
    {
        Id = Guid.CreateVersion7();
        DocumentId = documentId;
        Text = content;
        Hash = contentHash;
    }
}