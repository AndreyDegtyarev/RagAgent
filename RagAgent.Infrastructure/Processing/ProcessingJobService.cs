using MassTransit;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Processing;

public sealed class ProcessingJobService(
    IDocumentProcessingJobRepository repository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    IProcessingMessageFactory messageFactory)
    : IProcessingJobService
{
    public async Task CompleteStepAsync(Guid jobId, ProcessingStep nextStep, CancellationToken cancellationToken)
    {
        var job = await repository.GetByIdAsync(jobId, cancellationToken);

        job.Complete();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task FailStepAsync(Guid jobId, string error, CancellationToken cancellationToken)
    {
        var job = await repository.GetByIdAsync(jobId, cancellationToken);

        job.Fail(error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task StartProcessingAsync(
        Guid documentId,
        ProcessingStep step,
        CancellationToken cancellationToken)
    {
        var job = DocumentProcessingJob.Create(
            documentId,
            step);

        job.Start();
        
        await repository.AddAsync(job, cancellationToken);

        var message = messageFactory.Create(job);
        await publishEndpoint.Publish(
            message,
            cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}