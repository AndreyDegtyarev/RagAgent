using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentChunkEmbeddingRepository : IDocumentChunkEmbeddingRepository
{
    private readonly RagDbContext _db;

    public DocumentChunkEmbeddingRepository(RagDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        ChunkEmbedding embedding,
        CancellationToken cancellationToken)
    {
        await _db.Set<ChunkEmbedding>().AddAsync(embedding, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid chunkId,
        string model,
        CancellationToken cancellationToken)
    {
        return await _db.Set<ChunkEmbedding>()
            .AnyAsync(
                e => e.ChunkId == chunkId && e.ModelName == model,
                cancellationToken);
    }
}