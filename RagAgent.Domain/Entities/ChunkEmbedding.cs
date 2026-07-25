using RagAgent.Domain.Interfaces;
using RagAgent.Domain.ValueObjects;

namespace RagAgent.Domain.Entities;

public class ChunkEmbedding : IDomainEntity
{
    public Guid Id { get; private set; }

    public Guid ChunkId { get; private set; }

    public EmbeddingModel Model { get; private set; } 
    
    public string ModelName { get; private set; }

    public int Dimensions { get; private set; }

    public Embedding Embedding { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }


    private ChunkEmbedding()
    {
    }


    private ChunkEmbedding(
        Guid chunkId,
        EmbeddingModel model,
        Embedding embedding)
    {
        Id = Guid.CreateVersion7();
        ChunkId = chunkId;
        Model = model;
        Embedding = embedding;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static ChunkEmbedding Create(
        Guid chunkId,
        EmbeddingModel model,
        Embedding embedding)
    {
        return new ChunkEmbedding(
            chunkId,
            model,
            embedding);
    }
}