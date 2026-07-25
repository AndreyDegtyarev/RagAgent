using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Entities;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public sealed class DocumentTextConfiguration : IEntityTypeConfiguration<DocumentText>
{
    public void Configure(EntityTypeBuilder<DocumentText> builder)
    {
        builder.HasKey(x => x.DocumentId);

        builder.Property(x => x.Text)
            .IsRequired();

        builder.Property(x => x.CharacterCount)
            .IsRequired();

        builder.Property(x => x.PageCount)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.ToTable("DocumentTexts");
    }
}
