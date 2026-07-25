using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using RagAgent.Application.Commands.UploadDocument;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure;

namespace RagAgent.Api.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController(UploadDocumentHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        
        var command = new UploadDocumentCommand(file.FileName, stream);
        var id = await handler.Handle(command, cancellationToken);
        
        return Accepted(new
        {
            DocumentId = id,
            Status = "Processing"
        });
    }
}