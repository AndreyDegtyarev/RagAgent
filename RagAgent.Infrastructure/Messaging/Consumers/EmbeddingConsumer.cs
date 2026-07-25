using MassTransit;
using RagAgent.Application.Abstractions.Processing.Embedding;
using RagAgent.Contracts.Events;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public sealed class EmbeddingConsumer(EmbeddingHandler handler) : IConsumer<EmbeddingRequested>
{
    public Task Consume(ConsumeContext<EmbeddingRequested> context)
    {
        return handler.HandleAsync(
            context.Message,
            context.CancellationToken);
    }
}