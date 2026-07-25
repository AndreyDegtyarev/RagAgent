using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentChunkEmbeddingRepository : IDocumentChunkEmbeddingRepository
{
    public Task AddAsync(ChunkEmbedding embedding, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid chunkId, string model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}