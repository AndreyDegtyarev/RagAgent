using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Persistence;

public interface IDocumentChunkRepository
{
    Task<IReadOnlyList<DocumentChunk>> GetWithoutEmbeddingAsync(
            Guid documentId,
            string model,
            CancellationToken cancellationToken);
}