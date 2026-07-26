namespace RagAgent.Application.Abstractions;

public interface IFileInfoService
{
    public string GetContentType(Stream stream);
    
    string GetPath(Guid documentId, string originalFileName);
}