using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Persistence;

public interface IDocumentChunkEmbeddingRepository
{
    Task AddAsync(
        ChunkEmbedding embedding,
        CancellationToken cancellationToken);


    Task<bool> ExistsAsync(
        Guid chunkId,
        string model,
        CancellationToken cancellationToken);
}