using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Repositories;

public class DocumentProcessingJobRepository(RagDbContext dbContext) : IDocumentProcessingJobRepository
{
    public async Task AddAsync(
        DocumentProcessingJob job,
        CancellationToken cancellationToken)
    {
        await dbContext.DocumentProcessingJobs
            .AddAsync(
                job,
                cancellationToken);
    }


    public async Task<DocumentProcessingJob> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var job =  await dbContext.DocumentProcessingJobs
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return job ?? throw new KeyNotFoundException(
            $"DocumentProcessingJob '{id}' was not found.");
    }
}