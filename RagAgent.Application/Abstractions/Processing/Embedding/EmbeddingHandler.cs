using RagAgent.Application.Abstractions.AI;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Contracts.Events;
using RagAgent.Domain.Entities;
using RagAgent.Domain.Enums;

namespace RagAgent.Application.Abstractions.Processing.Embedding;

public sealed class EmbeddingHandler(
    IDocumentChunkRepository chunkRepository,
    IDocumentChunkEmbeddingRepository embeddingRepository,
    IEmbeddingGenerator generator,
    IProcessingJobService jobService,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        EmbeddingRequested request,
        CancellationToken cancellationToken)
    {
        try
        {
            var chunks =
                await chunkRepository.GetWithoutEmbeddingAsync(
                    request.DocumentId,
                    generator.Model.Name,
                    cancellationToken);


            const int batchSize = 50;


            foreach (var batch in chunks.Chunk(batchSize))
            {
                foreach (var chunk in batch)
                {
                    var embedding =
                        await generator.GenerateAsync(
                            chunk.Text,
                            cancellationToken);


                    var entity =
                        ChunkEmbedding.Create(
                            chunk.Id,
                            generator.Model,
                            embedding);


                    await embeddingRepository.AddAsync(
                        entity,
                        cancellationToken);
                }
                
                await unitOfWork.SaveChangesAsync(
                    cancellationToken);
            }
            
            await jobService.CompleteStepAsync(
                request.JobId,
                ProcessingStep.Embedding,
                cancellationToken);
            
            await jobService.StartProcessingAsync(
                request.DocumentId,
                ProcessingStep.VectorIndexing,
                cancellationToken);
        }
        catch (Exception ex)
        {
            await jobService.FailStepAsync(
                request.JobId,
                ex.Message,
                cancellationToken);

            throw;
        }
    }
}