namespace RagAgent.Domain.Entities;

public class DocumentText
{
    private DocumentText()
    {
    }

    public DocumentText(Guid documentId, string text, int characterCount, int pageCount)
    {
        DocumentId = documentId;
        Text = text;
        CharacterCount = characterCount;
        PageCount = pageCount;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid DocumentId { get; private set; }

    public string Text { get; private set; } = default!;

    public int CharacterCount { get; private set; }

    public int PageCount { get; private set; }

    public DateTime CreatedAt { get; private set; }
}