using RagAgent.Domain.ValueObjects;

namespace RagAgent.Application.Abstractions.AI;

public interface IEmbeddingGenerator
{
    EmbeddingModel Model { get; }

    Task<Embedding> GenerateAsync(
        string text,
        CancellationToken cancellationToken);
}