using MassTransit;
using RagAgent.Contracts.Events;

namespace RagAgent.Infrastructure.Messaging.Consumers;

public class DocumentUploadedConsumer : IConsumer<DocumentUploaded>
{
    public async Task Consume(ConsumeContext<DocumentUploaded> context)
    {
        var documentId = context.Message.DocumentId;
        
        Console.WriteLine(
            $"Processing document {documentId}");


        await Task.CompletedTask;
    }
}