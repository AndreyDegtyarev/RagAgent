using RagAgent.Application.Abstractions.Persistence;

namespace RagAgent.Infrastructure.Persistence;

public class EfUnitOfWork(RagDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(
            cancellationToken);
    }
}