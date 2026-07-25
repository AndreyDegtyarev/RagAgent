using System.Security.Cryptography;
using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Infrastructure.Persistence;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public class ChunkDocumentConsumer 
    : IConsumer<DocumentUploaded>
{
    private readonly RagDbContext _db;
    private readonly ITextChunker _chunker;
    private readonly IPublishEndpoint _publish;


    public ChunkDocumentConsumer(
        RagDbContext db,
        ITextChunker chunker,
        IPublishEndpoint publish)
    {
        _db = db;
        _chunker = chunker;
        _publish = publish;
    }


    public async Task Consume(
        ConsumeContext<DocumentUploaded> context)
    {
        var document =
            await _db.Documents
                .FirstAsync(
                    x => x.Id == context.Message.DocumentId);


        document.StartProcessing();


        // временно вместо PDF
        var text =
            """
            RAG is a technique
            that combines retrieval
            and generation.
            """;


        var chunks =
            _chunker.Split(text);


        foreach(var textChunk in chunks)
        {
            var chunk =
                new DocumentChunk(
                    document.Id,
                    textChunk,
                    CreateHash(textChunk));


            _db.Chunks.Add(chunk);


            await _publish.Publish(
                new DocumentChunkCreated(
                    chunk.Id,
                    document.Id));
        }


        await _db.SaveChangesAsync();
    }


    private static string CreateHash(string value)
    {
        using var sha =
            SHA256.Create();


        var bytes =
            Encoding.UTF8.GetBytes(value);


        return Convert.ToHexString(
            sha.ComputeHash(bytes));
    }
}