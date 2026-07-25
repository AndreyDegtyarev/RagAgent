using RagAgent.Domain.Enums;

namespace RagAgent.Domain.ValueObjects;

public sealed class EmbeddingModel
{
    private EmbeddingModel()
    {
    }


    private EmbeddingModel(
        string name,
        int dimensions,
        EmbeddingProvider provider)
    {
        Name = name;
        Dimensions = dimensions;
        Provider = provider;
    }


    public string Name { get; } = default!;
    
    public int Dimensions { get; }
    
    public EmbeddingProvider Provider { get; }


    public static EmbeddingModel Create(
        string name,
        int dimensions,
        EmbeddingProvider provider)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Model name is required.");


        if (dimensions <= 0)
            throw new ArgumentException(
                "Dimensions must be positive.");


        return new EmbeddingModel(
            name,
            dimensions,
            provider);
    }


    public override bool Equals(
        object? obj)
    {
        if (obj is not EmbeddingModel other)
            return false;


        return Name == other.Name
               && Dimensions == other.Dimensions
               && Provider == other.Provider;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Dimensions, Provider);
    }
}