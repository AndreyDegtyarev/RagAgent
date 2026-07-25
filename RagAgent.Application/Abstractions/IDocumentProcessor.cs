namespace RagAgent.Application.Abstractions;

public interface IDocumentProcessor
{
    Task ProcessAsync(
        int documentId,
        CancellationToken cancellationToken);
}