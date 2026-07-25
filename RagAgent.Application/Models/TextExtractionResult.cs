namespace RagAgent.Application.Models;

public sealed class TextExtractionResult
{
    public string Text { get; init; } = default!;

    public int Pages { get; init; }

    public TimeSpan Duration { get; init; }
}