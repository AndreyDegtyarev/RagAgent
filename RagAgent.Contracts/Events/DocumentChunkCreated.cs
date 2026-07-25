using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public sealed record DocumentChunkCreated(Guid ChunkId, Guid DocumentId) : IMessage;