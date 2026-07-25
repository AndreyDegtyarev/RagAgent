using MassTransit;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Services;

public sealed class TextExtractionConsumer(
    IFileStorage fileStorage,
    ITextExtractor textExtractor,
    IDocumentTextRepository documentTextRepository,
    IProcessingJobService jobService)
    : IConsumer<TextExtractionRequested>
{
    public async Task Consume(ConsumeContext<TextExtractionRequested> context)
    {
        var message = context.Message;

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

        await jobService.StartProcessingAsync(
            message.DocumentId,
            ProcessingStep.Chunking, 
            context.CancellationToken);
    }
}