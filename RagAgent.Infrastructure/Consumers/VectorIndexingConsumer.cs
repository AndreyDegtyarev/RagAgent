using MassTransit;
using Microsoft.Extensions.Logging;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Consumers;

public sealed class VectorIndexingConsumer(
    IProcessingJobService processingJobService,
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork,
    ILogger<VectorIndexingConsumer> logger)
    : IConsumer<VectorIndexingRequested>
{
    public async Task Consume(ConsumeContext<VectorIndexingRequested> context)
    {
        var message = context.Message;
        logger.LogInformation("Starting vector indexing for document {DocumentId}", message.DocumentId);

        await processingJobService.StartProcessingAsync(
            message.DocumentId,
            ProcessingStep.VectorIndexing,
            context.CancellationToken);
        
        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        var document = await documentRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (document != null)
        {
            document.MarkCompleted();
        }
        
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Completed vector indexing for document {DocumentId}", message.DocumentId);
    }
}
