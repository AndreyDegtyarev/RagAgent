using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RagAgent.Application.Abstractions;
using RagAgent.Application.Abstractions.Persistence;
using RagAgent.Application.Abstractions.Processing;
using RagAgent.Application.Abstractions.Processing.Embedding;
using RagAgent.Application.Commands.UploadDocument;
using RagAgent.Infrastructure.AI.Ollama;
using RagAgent.Infrastructure.Consumers;
using RagAgent.Infrastructure.Messaging;
using RagAgent.Infrastructure.Persistence;
using RagAgent.Infrastructure.Processing;
using RagAgent.Infrastructure.Repositories;
using RagAgent.Infrastructure.Services;

namespace RagAgent.Infrastructure.Extensions;

public static class ImportServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OllamaOptions>(configuration.GetSection(OllamaOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        AddServices(services);
        
        ConfigureDatabase(services, configuration);
        
        ConfigureRabbitMq(services, configuration);
        
        return services;
    }

    private static void ConfigureRabbitMq(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
            ?? new RabbitMqOptions();

        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            
            cfg.AddConsumer<DocumentFileStoredConsumer>();
            cfg.AddConsumer<ChunkDocumentConsumer>();
            cfg.AddConsumer<EmbeddingConsumer>();
            cfg.AddConsumer<TextExtractionConsumer>();
            cfg.AddConsumer<VectorIndexingConsumer>();

            cfg.AddEntityFrameworkOutbox<RagDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
    
            cfg.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });

                rabbit.ConfigureEndpoints(context);
            });
        });
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        services.AddDbContext<RagDbContext>(
            options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsql =>
                    {
                        npgsql.UseVector();
                    });
                
                
            });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ITextChunker, SimpleTextChunker>();
        services.AddScoped<UploadDocumentHandler>();
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IDocumentProcessingJobRepository, DocumentProcessingJobRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IProcessingMessageFactory, ProcessingMessageFactory>();
        services.AddScoped<IProcessingJobService, ProcessingJobService>();
        services.AddScoped<EmbeddingHandler>();
        services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
        services.AddScoped<IDocumentChunkEmbeddingRepository, DocumentChunkEmbeddingRepository>();
        services.AddScoped<IPdfTextExtractor, PdfPigTextExtractor>();
        services.AddScoped<ITextExtractor, PdfTextExtractor>();
        services.AddScoped<IDocumentTextRepository, DocumentTextRepository>();
    }
}