using RagAgent.Contracts.Abstractions;

namespace RagAgent.Contracts.Events;

public record DocumentUploaded(Guid DocumentId) : IMessage;