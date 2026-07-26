using Microsoft.EntityFrameworkCore;
using RagAgent.Application.Abstractions.AI;
using RagAgent.Infrastructure.AI.Ollama;
using RagAgent.Infrastructure.Extensions;
using RagAgent.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

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
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RagAgent API v1");
    });
}

app.UseSerilogRequestLogging();
app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<RagDbContext>();

await dbContext.Database.MigrateAsync();

await app.RunAsync();

