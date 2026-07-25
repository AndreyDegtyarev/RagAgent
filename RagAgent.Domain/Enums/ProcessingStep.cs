namespace RagAgent.Domain.Enums;

public enum ProcessingStep
{
    Upload = 0,

    TextExtraction = 1,

    Chunking = 2,

    Embedding = 3,

    VectorIndexing = 4
}