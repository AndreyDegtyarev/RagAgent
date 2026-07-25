using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : EntityConfigurationBase<Document>
{
    protected override void DoConfigure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(500);
        
        builder.HasIndex(x => new { x.Status, x.FileHash });
        
        builder.Property(x => x.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}