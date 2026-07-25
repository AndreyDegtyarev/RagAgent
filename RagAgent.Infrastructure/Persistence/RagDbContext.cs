using MassTransit;
using Microsoft.EntityFrameworkCore;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Persistence;

public class RagDbContext : DbContext
{
    public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentProcessingJob> DocumentProcessingJobs => Set<DocumentProcessingJob>();

    public RagDbContext(DbContextOptions<RagDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RagDbContext).Assembly);
        
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        
        base.OnModelCreating(modelBuilder);
    }
}