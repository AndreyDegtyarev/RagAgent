using RagAgent.Application.Models;

namespace RagAgent.Application.Abstractions;

public interface ITextExtractor
{
    Task<TextExtractionResult> ExtractAsync(
        Stream stream,
        CancellationToken cancellationToken);
}