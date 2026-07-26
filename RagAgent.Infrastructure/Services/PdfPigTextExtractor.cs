using RagAgent.Application.Abstractions;
using RagAgent.Application.Models;
using UglyToad.PdfPig;

namespace RagAgent.Infrastructure.Services;

public class PdfPigTextExtractor : IPdfTextExtractor
{
    public Task<IReadOnlyList<PageText>> ExtractAsync(
        Stream pdf,
        CancellationToken cancellationToken)
    {
        using var document = PdfDocument.Open(pdf);
        var result = document
             .GetPages()
             .Select(page => new PageText(page.Number, page.Text))
             .ToList();

        return Task.FromResult<IReadOnlyList<PageText>>(result);
    }
}