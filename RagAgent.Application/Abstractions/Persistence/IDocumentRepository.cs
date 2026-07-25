using RagAgent.Domain.Entities;

namespace RagAgent.Application.Abstractions.Persistence;

public interface IDocumentRepository
{
    Task AddAsync(
        Document document,
        CancellationToken cancellationToken);

    Task<Document?> GetAsync(
        Guid id,
        CancellationToken cancellationToken);
}