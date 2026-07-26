namespace RagAgent.Application.Commands.UploadDocument;

public sealed record UploadDocumentCommand(
    string FileName,
    Stream File)
{
    public long FileSize => File.Length;
}