using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentChunkRepository : IDocumentChunkRepository
{
    public Task<IReadOnlyList<DocumentChunk>> GetWithoutEmbeddingAsync(Guid documentId, string model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}