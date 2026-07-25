using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Persistence;

public interface IDocumentTextRepository
{
    Task SaveAsync(
        DocumentText documentText,
        CancellationToken cancellationToken);

    Task<DocumentText?> GetAsync(
        Guid documentId,
        CancellationToken cancellationToken);
}