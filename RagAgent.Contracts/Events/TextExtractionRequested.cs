using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public sealed record TextExtractionRequested(Guid DocumentId) : IMessage;