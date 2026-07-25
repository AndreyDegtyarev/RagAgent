using MassTransit;
using Microsoft.Extensions.Logging;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Consumers;

public sealed class TextExtractionConsumer(
    IFileStorage fileStorage,
    ITextExtractor textExtractor,
    IDocumentTextRepository documentTextRepository,
    IProcessingJobService jobService,
    ILogger<TextExtractionConsumer> logger)
    : IConsumer<TextExtractionRequested>
{
    public async Task Consume(ConsumeContext<TextExtractionRequested> context)
    {
        var message = context.Message;
        logger.LogInformation("Starting text extraction for document {DocumentId}", message.DocumentId);

        await using var stream = await fileStorage.OpenReadAsync(
                message.DocumentId,
                context.CancellationToken);

        var text =
            await textExtractor.ExtractAsync(
                stream,
                context.CancellationToken);

        var documentText = new DocumentText(
            message.DocumentId,
            text.Text,
            text.Text.Length,
            text.Pages);

        await documentTextRepository.SaveAsync(
            documentText,
            context.CancellationToken);

        logger.LogInformation("Extracted {Length} characters from {Pages} pages for document {DocumentId}", text.Text.Length, text.Pages, message.DocumentId);

        await jobService.StartProcessingAsync(
            message.DocumentId,
            ProcessingStep.Chunking, 
            context.CancellationToken);
    }
}