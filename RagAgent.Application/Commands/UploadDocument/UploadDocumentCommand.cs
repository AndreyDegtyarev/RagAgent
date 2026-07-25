namespace RagAgent.Application.Commands.UploadDocument;

public sealed record UploadDocumentCommand(
    string FileName,
    Stream File);