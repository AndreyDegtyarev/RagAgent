using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public class DocumentProcessingJobConfiguration
    : EntityConfigurationBase<DocumentProcessingJob>
{
    protected override void DoConfigure(EntityTypeBuilder<DocumentProcessingJob> builder)
    {
        builder.HasKey(
            x => x.Id);


        builder.Property(
                x => x.Step)
            .HasConversion<int>();


        builder.Property(
                x => x.Status)
            .HasConversion<int>();


        builder.Property(
                x => x.Error)
            .HasMaxLength(2000);


        builder.HasIndex(
            x => new
            {
                x.DocumentId,
                x.Step,
                x.Status
            });
        
        builder.HasIndex(x => x.CorrelationId);
    }
}