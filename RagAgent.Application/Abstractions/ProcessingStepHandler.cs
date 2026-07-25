namespace RagAgent.Application.Abstractions;

public abstract class ProcessingStepHandler<TMessage>
{
    public Task Handle(TMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    protected abstract Task ExecuteAsync(TMessage message, CancellationToken ct);
}