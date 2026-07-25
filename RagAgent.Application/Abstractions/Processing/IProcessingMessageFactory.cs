using RagAgent.Contracts.Abstractions;
using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Processing;

public interface IProcessingMessageFactory
{
    IMessage Create(DocumentProcessingJob job);
}