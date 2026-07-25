namespace RagAgent.Domain.Entities;

public class DocumentText
{
    public Guid DocumentId { get; private set; }

    public string Text { get; private set; } = default!;

    public int CharacterCount { get; private set; }

    public int PageCount { get; private set; }

    public DateTime CreatedAt { get; private set; }
}