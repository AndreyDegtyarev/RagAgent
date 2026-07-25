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

public class ChunkDocumentConsumer : IConsumer<ChunkingRequested>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentTextRepository _documentTextRepository;
    private readonly IDocumentChunkRepository _chunkRepository;
    private readonly IDocumentProcessingJobRepository _jobRepository;
    private readonly IProcessingJobService _jobService;
    private readonly ITextChunker _chunker;
    private readonly IUnitOfWork _unitOfWork;

    public ChunkDocumentConsumer(
        IDocumentRepository documentRepository,
        IDocumentTextRepository documentTextRepository,
        IDocumentChunkRepository chunkRepository,
        IDocumentProcessingJobRepository jobRepository,
        IProcessingJobService jobService,
        ITextChunker chunker,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _documentTextRepository = documentTextRepository;
        _chunkRepository = chunkRepository;
        _jobRepository = jobRepository;
        _jobService = jobService;
        _chunker = chunker;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<ChunkingRequested> context)
    {
        var message = context.Message;

        var job = await _jobRepository.GetByIdAsync(message.JobId, context.CancellationToken);
        job.Start();
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        var document = await _documentRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (document == null)
            throw new InvalidOperationException($"Document {message.DocumentId} not found.");

        document.StartProcessing();

        var documentText = await _documentTextRepository.GetAsync(message.DocumentId, context.CancellationToken);
        if (documentText == null)
            throw new InvalidOperationException($"Extracted text for document {message.DocumentId} not found.");

        var chunks = _chunker.Split(documentText.Text);

        foreach (var textChunk in chunks)
        {
            var chunk = new DocumentChunk(
                document.Id,
                textChunk,
                CreateHash(textChunk));

            await _chunkRepository.AddAsync(chunk, context.CancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        await _jobService.CompleteStepAsync(
            message.JobId,
            ProcessingStep.Chunking,
            context.CancellationToken);

        await _jobService.StartProcessingAsync(
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