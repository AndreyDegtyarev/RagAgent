using MassTransit;
using RagAgent.Application.Abstractions;

namespace RagAgent.Infrastructure.Messaging;

public class MassTransitEventPublisher(IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken)
    {
        return publishEndpoint.Publish(
            message,
            cancellationToken);
    }
}