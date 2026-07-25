using RagAgent.Domain.Enums;
using RagAgent.Domain.Interfaces;

namespace RagAgent.Domain.Entities;

public class DocumentProcessingJob : IDomainEntity
{
    private DocumentProcessingJob()
    {
    }

    private DocumentProcessingJob(
        Guid documentId,
        ProcessingStep step)
    {
        Id = Guid.CreateVersion7();
        DocumentId = documentId;
        Step = step;
        Status = ProcessingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static DocumentProcessingJob Create(
        Guid documentId,
        ProcessingStep step)
    {
        return new DocumentProcessingJob(documentId, step);
    }

    public Guid Id { get; private set; }

    public Guid DocumentId { get; private set; }

    public ProcessingStep Step { get; private set; }

    public ProcessingStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? Error { get; private set; }

    public Guid CorrelationId { get; private set; }
    
    public uint RetryCount { get; private set; }

    public void Start()
    {
        if (Status != ProcessingStatus.Pending)
            throw new InvalidOperationException();

        Status = ProcessingStatus.Running;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ProcessingStatus.Running)
            throw new InvalidOperationException();

        Status = ProcessingStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string error)
    {
        Status = ProcessingStatus.Failed;
        Error = error;
        RetryCount++;
    }
}