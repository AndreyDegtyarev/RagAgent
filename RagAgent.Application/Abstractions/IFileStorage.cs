namespace RagAgent.Application.Abstractions;

public interface IFileStorage
{
    Task SaveAsync(
        Guid documentId,
        Stream content,
        CancellationToken cancellationToken);


    Task<Stream> OpenReadAsync(
        Guid documentId,
        CancellationToken cancellationToken);

    Task DeleteAsync(CancellationToken cancellationToken);
}