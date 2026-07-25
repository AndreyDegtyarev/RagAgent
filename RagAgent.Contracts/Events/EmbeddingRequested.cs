using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public sealed record EmbeddingRequested(
    Guid JobId,
    Guid DocumentId) : IMessage;