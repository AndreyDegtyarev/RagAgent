namespace RagAgent.Infrastructure.AI.Ollama;

internal sealed class OllamaEmbeddingResponse
{
    public float[] Embedding { get; set; }
        = [];
}