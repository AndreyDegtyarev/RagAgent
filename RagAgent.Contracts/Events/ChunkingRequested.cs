using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public sealed record ChunkingRequested(
    Guid JobId,
    Guid DocumentId) : IMessage;