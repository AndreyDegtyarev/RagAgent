using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;

namespace RagAgent.Application.Commands.UploadDocument;

public class UploadDocumentHandler
{
    private readonly IDocumentRepository _documentRepository;

    private readonly IFileStorage _storage;

    private readonly IEventPublisher _publisher;

    private readonly IUnitOfWork _unitOfWork;


    public UploadDocumentHandler(
        IDocumentRepository documentRepository,
        IFileStorage storage,
        IEventPublisher publisher,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _storage = storage;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
    }


    public async Task<Guid> Handle(UploadDocumentCommand command, CancellationToken ct)
    {
        var document = new Document(command.FileName);
        
        await _documentRepository.AddAsync(document, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _storage.SaveAsync(document.Id, command.File, ct);

        document.MarkFileStored();

        await _publisher.PublishAsync(new DocumentFileStored(document.Id), ct);
        await _unitOfWork.SaveChangesAsync(ct);


        return document.Id;
    }
}