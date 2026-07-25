using MassTransit;
using Microsoft.Extensions.Logging;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Consumers;

public class DocumentFileStoredConsumer(
    IProcessingJobService processingJobService,
    ILogger<DocumentFileStoredConsumer> logger)
    : IConsumer<DocumentFileStored>
{
    public async Task Consume(ConsumeContext<DocumentFileStored> context)
    {
        var message = context.Message;
        logger.LogInformation("Document file stored event received for document {DocumentId}. Initiating text extraction.", message.DocumentId);

        await processingJobService.StartProcessingAsync(
            message.DocumentId,
            ProcessingStep.TextExtraction,
            context.CancellationToken);
    }
}