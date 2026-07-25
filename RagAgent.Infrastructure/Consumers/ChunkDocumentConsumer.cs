using System.Security.Cryptography;
using System.Text;
using MassTransit;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public class ChunkDocumentConsumer(
    IDocumentRepository documentRepository,
    IDocumentTextRepository documentTextRepository,
    IDocumentChunkRepository chunkRepository,
    IProcessingJobService jobService,
    ITextChunker chunker,
    IUnitOfWork unitOfWork)
    : IConsumer<ChunkingRequested>
{
    public async Task Consume(ConsumeContext<ChunkingRequested> context)
    {
        var message = context.Message;

        var document = await documentRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (document == null)
            throw new InvalidOperationException($"Document {message.DocumentId} not found.");

        document.StartProcessing();

        var documentText = await documentTextRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (documentText == null)
            throw new InvalidOperationException($"Extracted text for document {message.DocumentId} not found.");

        var chunks = chunker.Split(documentText.Text);

        foreach (var textChunk in chunks)
        {
            var chunk = new DocumentChunk(
                document.Id,
                textChunk,
                CreateHash(textChunk));

            await chunkRepository.AddAsync(chunk, context.CancellationToken);
        }

        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        await jobService.CompleteStepAsync(
            message.JobId,
            ProcessingStep.Chunking,
            context.CancellationToken);

        await jobService.StartProcessingAsync(
            message.DocumentId,
            ProcessingStep.Embedding,
            context.CancellationToken);
    }

    private static string CreateHash(string value)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }
}