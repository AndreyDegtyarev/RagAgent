namespace RagAgent.Domain.ValueObjects;

public sealed class Embedding
{
    private Embedding()
    {
    }


    private Embedding(float[] values)
    {
        Values = values;
    }


    public float[] Values { get; private set; }
        = Array.Empty<float>();


    public int Dimensions =>
        Values.Length;


    public static Embedding Create(
        EmbeddingModel model,
        float[] values)
    {
        if (values.Length != model.Dimensions)
        {
            throw new InvalidOperationException("Invalid vector dimensions");
        }
        
        return new Embedding(values);
    }
    
    internal static Embedding Create(float[] values)
    {
        return new Embedding(values);
    }
}