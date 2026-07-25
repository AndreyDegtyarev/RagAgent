using RagAgent.Application.Abstractions;

namespace RagAgent.Infrastructure.Services;

public class SimpleTextChunker : ITextChunker
{
    public IEnumerable<string> Split(string text)
    {
        const int size = 500;
        
        for(var i = 0; i < text.Length; i += size)
        {
            yield return text.Substring(
                i,
                Math.Min(
                    size,
                    text.Length - i));
        }
    }
}