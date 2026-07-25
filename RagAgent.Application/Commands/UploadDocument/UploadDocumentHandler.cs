using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;

namespace RagAgent.Application.Commands.UploadDocument;

public class UploadDocumentHandler(
    IDocumentRepository documentRepository,
    IFileStorage storage,
    IEventPublisher publisher,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> Handle(UploadDocumentCommand command, CancellationToken ct)
    {
        var document = new Document(command.FileName);
        
        await documentRepository.AddAsync(document, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await storage.SaveAsync(document.Id, command.File, ct);

        document.MarkFileStored();

        await publisher.PublishAsync(new DocumentFileStored(document.Id), ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return document.Id;
    }
}