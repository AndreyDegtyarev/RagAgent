using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Repositories;

public sealed class DocumentTextRepository(RagDbContext dbContext) : IDocumentTextRepository
{
    public async Task SaveAsync(
        DocumentText documentText,
        CancellationToken cancellationToken)
    {
        await dbContext.DocumentTexts.AddAsync(documentText, cancellationToken);
    }

    public Task<DocumentText?> GetAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return dbContext.DocumentTexts
            .FirstOrDefaultAsync(x => x.DocumentId == documentId, cancellationToken);
    }
}
