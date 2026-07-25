using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.AI;
using RagAgent.Infrastructure.AI.Ollama;
using RagAgent.Infrastructure.Extensions;
using RagAgent.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddInfrastructure(configuration);

services.AddHttpClient<IEmbeddingGenerator,
    OllamaEmbeddingGenerator>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                configuration["Ollama:BaseUrl"]!);
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<RagDbContext>();

await dbContext.Database.MigrateAsync();

await app.RunAsync();

