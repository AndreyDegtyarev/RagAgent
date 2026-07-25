# RagAgent DevOps & Environment Setup Specification

This document provides a comprehensive guide for developers and DevOps agents to set up, configure, and run the **RagAgent** solution.

---

## 1. Architecture Overview

**RagAgent** is a Retrieval-Augmented Generation (RAG) pipeline built using the following stack:
* **Backend Framework**: .NET 10.0 Web API
* **Database**: PostgreSQL (v17) with the `pgvector` extension for vector search and storing embeddings
* **Message Broker**: RabbitMQ (v3.13) for event-driven async processing of uploaded documents
* **Orchestration**: MassTransit with Entity Framework Core Outbox
* **AI Embeddings**: Ollama running locally or as a service
* **Text Extraction**: PdfPig for PDF document processing

---

## 2. Prerequisites

Ensure the following tools are installed on the host system:
* **.NET 10.0 SDK** or newer
* **Docker Desktop** / **Docker Engine** with Docker Compose
* **Ollama** (v0.1.x or newer) for local embedding generation

---

## 3. Infrastructure Setup (Docker Compose)

The environment requires PostgreSQL (with `pgvector` support) and RabbitMQ. A pre-configured `docker-compose.yaml` is located in the `devops/` directory.

### Running the Services
To start the database and message broker, run:
```bash
docker-compose -f devops/docker-compose.yaml up -d
```

### Infrastructure Details

| Service | Image | Internal Port | Host Port | Credentials | Purpose |
| :--- | :--- | :---: | :---: | :--- | :--- |
| **PostgreSQL** | `pgvector/pgvector:pg17` | `5432` | `5432` | `User: raguser`<br>`Pass: ragpassword`<br>`DB: ragdb` | Relational storage + Vector indexing |
| **RabbitMQ** | `rabbitmq:3.13-management` | `5672`<br>`15672` | `5672`<br>`15672` | `User: raguser`<br>`Pass: ragpassword` | Messaging & event queue + UI dashboard |

> [!WARNING]
> **Windows Volume Mounting Issue:**
> In `devops/docker-compose.yaml`, the volume mapping `- ./postgres/init.sql:/docker-entrypoint-initdb.d/init.sql` may fail or create an empty directory named `init.sql` if the file did not exist before running Docker Compose. 
> 
> **To fix this:** Ensure `devops/postgres/init.sql` is created as a file (even if empty) before starting the containers, or delete the accidental `init.sql/` directory and recreate it as a file.

---

## 4. Ollama Configuration & Model Pull

The RAG pipeline utilizes Ollama to generate vector embeddings.

### A. Install and Run Ollama
Download and run Ollama from the [official website](https://ollama.com/). Make sure it is running locally and listening on its default port: `http://localhost:11434`.

### B. Pull the Embedding Model
> [!IMPORTANT]
> **Configuration/Dimension Mismatch Warning:**
> * The default `appsettings.json` is configured to use the **`all-minilm`** model (`"Model": "all-minilm"`).
> * However, the backend code (`SupportedEmbeddingModels.MultilingualE5Base` in [SupportedEmbeddingModels.cs](file:///d:/AI_Train/RagAgent/RagAgent.Domain/Embeddings/SupportedEmbeddingModels.cs)) is hardcoded to validate and store embeddings using the **`multilingual-e5-base`** model (which has a 768-dimension output).
> * The PostgreSQL migration also configures the database schema for 768 dimensions (`type: "vector(768)"`).
> * **If you use `all-minilm`, the application will throw a `Vector dimensions mismatch` error on save (384 dimensions vs 768).**

#### Recommended Setup Action:
To align the configuration with the database and code:
1. **Pull the `multilingual-e5-base` model** in Ollama:
   ```bash
   ollama pull multilingual-e5-base
   ```
2. **Update `appsettings.json`** to use this model:
   ```json
   "Ollama": {
     "BaseUrl": "http://localhost:11434",
     "Model": "multilingual-e5-base"
   }
   ```

---

## 5. Application Run & Verification

Once Docker services are up and Ollama models are pulled, you can start the application.

### A. Database Migrations
On startup, the API is configured to run database migrations automatically:
```csharp
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<RagDbContext>();
await dbContext.Database.MigrateAsync();
```
If you prefer running migrations manually before starting the app, execute:
```bash
dotnet ef database update --project RagAgent.Infrastructure --startup-project RagAgent.Api
```

### B. Starting the API
Start the Web API from the root directory:
```bash
dotnet run --project RagAgent.Api
```
The application will start listening on:
* HTTP: `http://localhost:5000` (or the port defined in `launchSettings.json`)
* HTTPS: `https://localhost:5001` (or the port defined in `launchSettings.json`)

### C. Verifying the Setup
You can explore and test the endpoints using OpenAPI/Swagger documentation at:
`http://localhost:5000/openapi/v1.json` or `https://localhost:5001/swagger/index.html` (depending on environment configuration).

Alternatively, upload a document using `curl`:
```bash
curl -X POST -F "file=@your_document.pdf" http://localhost:5000/api/documents
```
Expected API Response:
```json
{
  "documentId": "guid-here",
  "status": "Processing"
}
```
You can monitor background consumer execution in your API console or inspect RabbitMQ queues via the management dashboard: `http://localhost:15672/` (User/Password: `raguser/ragpassword`).
