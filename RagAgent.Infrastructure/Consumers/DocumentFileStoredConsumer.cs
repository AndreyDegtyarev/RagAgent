using MassTransit;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public class DocumentFileStoredConsumer(
    IProcessingJobService processingJobService)
    : IConsumer<DocumentFileStored>
{
    public async Task Consume(ConsumeContext<DocumentFileStored> context)
    {
        await processingJobService.StartProcessingAsync(
            context.Message.DocumentId,
            ProcessingStep.TextExtraction,
            context.CancellationToken);
    }
}