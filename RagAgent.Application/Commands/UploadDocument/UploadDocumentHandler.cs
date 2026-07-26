using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;

namespace RagAgent.Application.Commands.UploadDocument;

public class UploadDocumentHandler(
    IDocumentRepository documentRepository,
    IFileStorage storage,
    IEventPublisher publisher,
    IFileInfoService  fileInfoService,
    IUnitOfWork unitOfWork)
{
    public async Task<Guid> Handle(UploadDocumentCommand command, CancellationToken ct)
    {
        var contentType = fileInfoService.GetContentType(command.File);
        var document = new Document(command.FileName, contentType);
        var storagePath = fileInfoService.GetPath(document.Id, command.FileName);
        
        document.SetStoragePath(storagePath);
        
        await documentRepository.AddAsync(document, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await storage.SaveAsync(document.Id, command.File, ct);

        document.MarkFileStored();

        await publisher.PublishAsync(new DocumentFileStored(document.Id), ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return document.Id;
    }
}