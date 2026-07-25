using RagAgent.Domain.Enums;

namespace RagAgent.Application.Abstractions.Processing;

public interface IProcessingJobService
{
    Task CompleteStepAsync(
        Guid jobId,
        ProcessingStep nextStep,
        CancellationToken cancellationToken);

    Task FailStepAsync(
        Guid jobId,
        string error,
        CancellationToken cancellationToken);
    
    Task StartProcessingAsync(
        Guid documentId,
        ProcessingStep step,
        CancellationToken cancellationToken);
}