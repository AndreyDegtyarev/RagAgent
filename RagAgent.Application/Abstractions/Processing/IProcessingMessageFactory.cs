using RagAgent.Contracts.Abstractions;
using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Processing;

public interface IProcessingMessageFactory
{
    object Create(DocumentProcessingJob job);
}