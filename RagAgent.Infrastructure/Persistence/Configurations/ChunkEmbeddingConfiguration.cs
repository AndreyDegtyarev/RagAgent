using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence.Converters;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public class ChunkEmbeddingConfiguration : EntityConfigurationBase<ChunkEmbedding>
{
    protected override void DoConfigure(EntityTypeBuilder<ChunkEmbedding> builder)
    {
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)")
            .HasConversion<EmbeddingConverter>();
        
        builder.OwnsOne(
            x => x.Model,
            model =>
            {
                model.Property(x => x.Name)
                    .HasColumnName("ModelName")
                    .HasMaxLength(100);


                model.Property(x => x.Dimensions)
                    .HasColumnName("Dimensions");


                model.Property(x => x.Provider)
                    .HasColumnName("Provider")
                    .HasConversion<int>();
            });
        
        builder
            .HasIndex(x => new { x.ChunkId, x.ModelName })
            .IsUnique();
    }
}