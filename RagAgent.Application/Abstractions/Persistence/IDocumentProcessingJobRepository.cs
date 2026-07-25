using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Persistence;

public interface IDocumentProcessingJobRepository
{
    Task AddAsync(
        DocumentProcessingJob job,
        CancellationToken cancellationToken);


    Task<DocumentProcessingJob> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}