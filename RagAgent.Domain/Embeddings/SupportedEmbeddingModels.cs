using RagAgent.Domain.Enums;
using RagAgent.Domain.ValueObjects;

namespace RagAgent.Domain.Embeddings;

public static class SupportedEmbeddingModels
{
    public static readonly EmbeddingModel
        OpenAiTextEmbedding3Small =
            EmbeddingModel.Create(
                "text-embedding-3-small",
                1536,
                EmbeddingProvider.OpenAI);


    public static readonly EmbeddingModel
        MultilingualE5Base =
            EmbeddingModel.Create(
                "multilingual-e5-base",
                768,
                EmbeddingProvider.Ollama);
    
    public static readonly EmbeddingModel
        AllMinilm =
            EmbeddingModel.Create(
                "all-minilm",
                384,
                EmbeddingProvider.Ollama);
}