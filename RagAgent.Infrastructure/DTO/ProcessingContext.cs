using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.DTO;

public sealed class ProcessingContext
{
    public Document Document { get; init; }

    public DocumentProcessingJob Job { get; init; }

    public CancellationToken CancellationToken { get; init; }
}