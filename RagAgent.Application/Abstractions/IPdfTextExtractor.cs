using RagAgent.Application.Models;

namespace RagAgent.Application.Abstractions;

public interface IPdfTextExtractor
{
    Task<IReadOnlyList<PageText>> ExtractAsync(
        Stream pdf,
        CancellationToken cancellationToken);
}