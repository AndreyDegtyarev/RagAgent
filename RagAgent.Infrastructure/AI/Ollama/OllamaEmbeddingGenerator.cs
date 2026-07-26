using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RagAgent.Application.Abstractions.AI;
using RagAgent.Domain.Embeddings;
using RagAgent.Domain.ValueObjects;

namespace RagAgent.Infrastructure.AI.Ollama;


public sealed class OllamaEmbeddingGenerator(
    HttpClient httpClient,
    IOptions<OllamaOptions> options) : IEmbeddingGenerator
{
    private readonly OllamaOptions _options = options.Value;

    public EmbeddingModel Model =>
        SupportedEmbeddingModels.AllMinilm;
    

    public async Task<Embedding> GenerateAsync(
        string text,
        CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _options.Model,
            input = text
        };


        var response =
            await httpClient.PostAsJsonAsync(
                "/api/embed",
                request,
                cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>(
                    cancellationToken);
        
        if (result?.Embeddings == null ||  result.Embeddings.Length == 0)
        {
            throw new InvalidOperationException("Ollama returned empty embedding.");
        }
        
        return Embedding.Create(Model, result.Embeddings[0]);
    }
}