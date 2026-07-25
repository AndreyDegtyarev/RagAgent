using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;
using RagAgent.Domain.Embeddings;
using RagAgent.Domain.ValueObjects;

namespace RagAgent.Infrastructure.Persistence.Converters;

public class EmbeddingConverter() : ValueConverter<Embedding?, Vector?>(embedding =>
        embedding == null
            ? null
            : new Vector(embedding.Values.ToArray()),
    vector =>
        vector == null
            ? null
            : EmbeddingMapper.FromOllama(vector.ToArray()));