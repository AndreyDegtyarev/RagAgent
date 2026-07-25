using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public class DocumentChunkConfiguration : EntityConfigurationBase<DocumentChunk>
{
    protected override void DoConfigure(EntityTypeBuilder<DocumentChunk> builder)
    {
        // builder.Property(x => x.Embedding)
        //     .HasConversion<EmbeddingConverter>()
        //     .HasColumnType("vector(1536)");
        
        builder.HasIndex(x => new { x.Order, x.Hash });
        builder.HasIndex(x => x.Hash);
    }
}