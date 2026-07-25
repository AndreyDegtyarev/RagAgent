using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public sealed record VectorIndexingRequested(
    Guid JobId,
    Guid DocumentId) : IMessage;