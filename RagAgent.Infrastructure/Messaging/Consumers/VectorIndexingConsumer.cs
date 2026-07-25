using MassTransit;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public sealed class VectorIndexingConsumer(
    IDocumentProcessingJobRepository jobRepository,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<VectorIndexingRequested>
{
    public async Task Consume(ConsumeContext<VectorIndexingRequested> context)
    {
        var message = context.Message;

        var job = await jobRepository.GetByIdAsync(message.JobId, context.CancellationToken);
        
        job.Start();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        var document = await documentRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (document != null)
        {
            document.MarkCompleted();
        }

        job.Complete();
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
