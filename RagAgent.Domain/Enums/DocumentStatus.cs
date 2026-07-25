namespace RagAgent.Domain.Enums;

public enum DocumentStatus
{
    Unknown = 0,
    
    Created = 1,

    FileStored = 2,

    Processing = 3,

    Completed = 4,

    Failed = 5
}