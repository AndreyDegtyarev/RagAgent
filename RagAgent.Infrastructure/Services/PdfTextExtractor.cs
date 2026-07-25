using System.Diagnostics;
using System.Text;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Models;

namespace RagAgent.Infrastructure.Services;

public sealed class PdfTextExtractor(IPdfTextExtractor pdfExtractor) : ITextExtractor
{
    public async Task<TextExtractionResult> ExtractAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var pages = await pdfExtractor.ExtractAsync(
            stream,
            cancellationToken);
            
        stopwatch.Stop();

        var sb = new StringBuilder();
        foreach (var page in pages)
        {
            sb.AppendLine(page.Text);
        }

        return new TextExtractionResult
        {
            Text = sb.ToString(),
            Pages = pages.Count,
            Duration = stopwatch.Elapsed
        };
    }
}
