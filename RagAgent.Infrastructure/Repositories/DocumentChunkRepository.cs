using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentChunkRepository : IDocumentChunkRepository
{
    private readonly RagDbContext _db;

    public DocumentChunkRepository(RagDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetWithoutEmbeddingAsync(
        Guid documentId,
        string model,
        CancellationToken cancellationToken)
    {
        var embeddedChunkIds = await _db.Set<ChunkEmbedding>()
            .Where(e => e.ModelName == model)
            .Select(e => e.ChunkId)
            .ToListAsync(cancellationToken);

        var chunks = await _db.Chunks
            .Where(c => c.DocumentId == documentId && !embeddedChunkIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return chunks;
    }

    public async Task AddAsync(
        DocumentChunk chunk,
        CancellationToken cancellationToken)
    {
        await _db.Chunks.AddAsync(chunk, cancellationToken);
    }
}