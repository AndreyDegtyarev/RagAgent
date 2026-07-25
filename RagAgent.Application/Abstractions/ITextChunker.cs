namespace RagAgent.Application.Abstractions;

public interface ITextChunker
{
    IEnumerable<string> Split(string text);
}