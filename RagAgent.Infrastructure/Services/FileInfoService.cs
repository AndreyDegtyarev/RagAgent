using MimeDetective;
using RagAgent.Application.Abstractions;

namespace RagAgent.Infrastructure.Services;

public class FileInfoService : IFileInfoService
{
    public string GetContentType(Stream stream)
    {
        var inspector = new ContentInspectorBuilder
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();

        var results = inspector.Inspect(stream);

        return results.FirstOrDefault()?.Definition.File.MimeType ??  "unknown";
    }

    public string GetPath(Guid documentId, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var path = $"documents/{documentId:N}{extension}";

        return path;
    }
}