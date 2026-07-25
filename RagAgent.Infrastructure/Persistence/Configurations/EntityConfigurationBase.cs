using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RagAgent.Domain.Interfaces;

namespace RagAgent.Infrastructure.Persistence.Configurations;

public abstract class EntityConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity: class, IDomainEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        DoConfigure(builder);
    }
    
    protected abstract void DoConfigure(EntityTypeBuilder<TEntity> builder);
}