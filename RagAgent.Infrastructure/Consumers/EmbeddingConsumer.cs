using MassTransit;
using Microsoft.Extensions.Logging;
using RagAgent.Application.Abstractions.Processing.Embedding;
using RagAgent.Contracts.Events;

namespace RagAgent.Infrastructure.Consumers;

public sealed class EmbeddingConsumer(
    EmbeddingHandler handler,
    ILogger<EmbeddingConsumer> logger)
    : IConsumer<EmbeddingRequested>
{
    public async Task Consume(ConsumeContext<EmbeddingRequested> context)
    {
        logger.LogInformation("Processing embedding request for document {DocumentId}", context.Message.DocumentId);
        await handler.HandleAsync(
            context.Message,
            context.CancellationToken);
    }
}