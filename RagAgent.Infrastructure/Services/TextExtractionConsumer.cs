using MassTransit;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Services;

public sealed class TextExtractionConsumer(
    IDocumentProcessingJobRepository jobRepository,
    IFileStorage fileStorage,
    ITextExtractor textExtractor,
    IDocumentTextRepository documentTextRepository,
    IEventPublisher publisher,
    IUnitOfWork unitOfWork)
    : IConsumer<TextExtractionRequested>
{
    public async Task Consume(ConsumeContext<TextExtractionRequested> context)
    {
        var message = context.Message;

        var job = await jobRepository.GetByIdAsync(
            message.JobId,
            context.CancellationToken);

        if (job == null)
            throw new InvalidOperationException(
                $"Job {message.JobId} not found.");

        job.Start();

        await unitOfWork.SaveChangesAsync(
            context.CancellationToken);

        await using var stream =
            await fileStorage.OpenReadAsync(
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

        job.Complete();

        var chunkingJob = DocumentProcessingJob.Create(
                message.DocumentId,
                ProcessingStep.Chunking);

        await jobRepository.AddAsync(
            chunkingJob,
            context.CancellationToken);

        await publisher.PublishAsync(
            new ChunkingRequested(
                chunkingJob.Id,
                message.DocumentId),
            context.CancellationToken);

        await unitOfWork.SaveChangesAsync(
            context.CancellationToken);
    }
}