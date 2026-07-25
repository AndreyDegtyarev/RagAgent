using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public record DocumentFileStored(Guid DocumentId) : IMessage;