namespace RagAgent.Infrastructure.AI.Ollama;

internal sealed class OllamaEmbeddingResponse
{
    public float[][] Embeddings { get; set; }
        = [];
}