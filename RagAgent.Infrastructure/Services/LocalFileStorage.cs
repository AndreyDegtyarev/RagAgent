using Microsoft.Extensions.Configuration;
using RagAgent.Application.Abstractions;

namespace RagAgent.Infrastructure.Services;

public class LocalFileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string _folder = configuration["Storage:Folder"] ?? "uploads";
    
    public async Task SaveAsync(
        Guid documentId,
        Stream content,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_folder);
        
        var path = Path.Combine(_folder, $"{documentId}.pdf");
        await using var file = File.Create(path);
        
        await content.CopyToAsync(
            file,
            cancellationToken);
    }


    public Task<Stream> OpenReadAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var path =
            Path.Combine(
                _folder,
                $"{documentId}.pdf");


        Stream stream =
            File.OpenRead(path);


        return Task.FromResult(stream);
    }

    public Task DeleteAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}