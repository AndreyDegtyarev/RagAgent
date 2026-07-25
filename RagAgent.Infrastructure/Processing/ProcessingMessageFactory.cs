using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Abstractions;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Processing;

public class ProcessingMessageFactory : IProcessingMessageFactory
{
    public IMessage Create(DocumentProcessingJob job)
    {
        return job.Step switch
        {
            ProcessingStep.TextExtraction =>
                new TextExtractionRequested(job.Id, job.DocumentId),

            ProcessingStep.Chunking =>
                new ChunkingRequested(job.Id, job.DocumentId),

            ProcessingStep.Embedding =>
                new EmbeddingRequested(job.Id, job.DocumentId),

            _ => throw new NotSupportedException(job.Step.ToString())
        };
    }
}