using RagAgent.Domain.ValueObjects;

namespace RagAgent.Domain.Embeddings;

public static class EmbeddingMapper
{
    public static Embedding FromOllama(float[] values)
    {
        return Embedding.Create(values);
    }
}