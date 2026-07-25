namespace RagAgent.Application.Models;

public sealed record EmbeddingProcessingResult(
    bool HasWork,
    int ProcessedItems);