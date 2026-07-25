using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly RagDbContext _db;


    public DocumentRepository(
        RagDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        Document document,
        CancellationToken cancellationToken)
    {
        await _db.Documents.AddAsync(
            document,
            cancellationToken);
    }


    public Task<Document?> GetAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _db.Documents
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}