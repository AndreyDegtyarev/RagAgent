using Microsoft.AspNetCore.Mvc;
using RagAgent.Application.Commands.UploadDocument;

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
    
    [HttpGet("test")]
    public async Task<IActionResult> TestWorkflow(string text, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(text);
        
        var command = new UploadDocumentCommand("Test", stream);
        var id = await handler.Handle(command, cancellationToken);
        
        return Accepted(new
        {
            DocumentId = id,
            Status = "Processing"
        });
    }
}