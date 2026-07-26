<#
.SYNOPSIS
  Startup script to orchestrate and launch all components of the RagAgent application.
.DESCRIPTION
  This script handles:
  1. Checking and installing prerequisites.
  2. Fixing potential Docker volume mount folder-vs-file issue.
  3. Starting PostgreSQL and RabbitMQ via Docker Compose.
  4. Checking Ollama and pulling the multilingual-e5-base embedding model.
  5. Running database migrations.
  6. Starting the backend Web API in a new window.
  7. Installing frontend dependencies and starting the Angular app in a new window.
  8. Opening the browser to the application frontend.
#>

$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "       RagAgent Application Orchestration     " -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# 1. Prerequisite check
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

$prereqs = @{
    "Docker" = "docker"
    "Dotnet SDK" = "dotnet"
    "Node.js/npm" = "npm"
}

$missingPrereqs = 0
foreach ($name in $prereqs.Keys) {
    $cmd = $prereqs[$name]
    if (-not (Get-Command $cmd -ErrorAction SilentlyContinue)) {
        Write-Host "[-] Missing prerequisite: $name ($cmd must be installed and in PATH)" -ForegroundColor Red
        $missingPrereqs++
    } else {
        Write-Host "[+] $name is available" -ForegroundColor Green
    }
}

if ($missingPrereqs -gt 0) {
    Write-Host "Please install all missing prerequisites and run the script again." -ForegroundColor Red
    exit 1
}

# 2. Fix PostgreSQL volume mount bug
$initSqlPath = Join-Path $PSScriptRoot "devops\postgres\init.sql"
if (Test-Path $initSqlPath) {
    $item = Get-Item $initSqlPath
    if ($item.PSIsContainer) {
        Write-Host "Detected directory 'init.sql' instead of file. Resolving Windows volume mount issue..." -ForegroundColor Yellow
        Remove-Item -Recurse -Force $initSqlPath
        [System.IO.File]::WriteAllText($initSqlPath, "-- Empty init.sql for postgres initialization`n")
        Write-Host "[+] Replaced init.sql folder with a file." -ForegroundColor Green
    }
} else {
    Write-Host "Creating init.sql file to prevent volume mount issue..." -ForegroundColor Yellow
    [System.IO.File]::WriteAllText($initSqlPath, "-- Empty init.sql for postgres initialization`n")
}

# 3. Spin up Docker Compose
Write-Host "Starting Docker containers (PostgreSQL & RabbitMQ)..." -ForegroundColor Yellow
docker compose -f "$PSScriptRoot\devops\docker-compose.yaml" up -d
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to start Docker containers. Exiting." -ForegroundColor Red
    exit 1
}

# 4. Wait for Docker containers to be healthy
Write-Host "Waiting for database and message broker to be healthy..." -ForegroundColor Yellow
$timeout = 60
$elapsed = 0
$healthy = $false

while ($elapsed -lt $timeout) {
    $postgresHealthy = (docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}unhealthy{{end}}' rag-postgres 2>$null) -eq "healthy"
    $rabbitmqHealthy = (docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}unhealthy{{end}}' rag-rabbitmq 2>$null) -eq "healthy"
    
    if ($postgresHealthy -and $rabbitmqHealthy) {
        Write-Host "[+] Infrastructure services are healthy!" -ForegroundColor Green
        $healthy = $true
        break
    }
    
    Write-Host "Waiting for services to boot... ($elapsed/$timeout s)" -ForegroundColor Gray
    Start-Sleep -Seconds 3
    $elapsed += 3
}

if (-not $healthy) {
    Write-Host "WARNING: Containers did not report healthy state in time. Proceeding anyway..." -ForegroundColor Yellow
}

# 5. Check Ollama and Model
Write-Host "Checking local Ollama status..." -ForegroundColor Yellow
try {
    $ollamaResponse = Invoke-RestMethod -Uri "http://localhost:11434/api/tags" -Method Get -TimeoutSec 3 -ErrorAction Stop
    Write-Host "[+] Ollama is running." -ForegroundColor Green
    
    $hasModel = $false
    if ($ollamaResponse.models) {
        foreach ($m in $ollamaResponse.models) {
            if ($m.name -like "*multilingual-e5-base*") {
                $hasModel = $true
                break
            }
        }
    }
    
    if (-not $hasModel) {
        Write-Host "Ollama model 'multilingual-e5-base' not found. Attempting to pull directly..." -ForegroundColor Yellow
        ollama pull multilingual-e5-base
        
        $checkList = ollama list
        if ($checkList -like "*multilingual-e5-base*") {
            Write-Host "[+] Model pulled successfully." -ForegroundColor Green
        } else {
            Write-Host "Direct pull failed. Attempting to pull community model 'qllama/multilingual-e5-base'..." -ForegroundColor Yellow
            ollama pull qllama/multilingual-e5-base
            
            $checkList = ollama list
            if ($checkList -like "*qllama/multilingual-e5-base*") {
                Write-Host "Copying 'qllama/multilingual-e5-base' to 'multilingual-e5-base'..." -ForegroundColor Yellow
                ollama cp qllama/multilingual-e5-base multilingual-e5-base
                Write-Host "[+] Model copied and ready as 'multilingual-e5-base'." -ForegroundColor Green
            } else {
                Write-Host "WARNING: Failed to pull community model. Ollama embeddings may fail." -ForegroundColor Red
            }
        }
    } else {
        Write-Host "[+] Ollama model 'multilingual-e5-base' is already present." -ForegroundColor Green
    }
} catch {
    Write-Host "WARNING: Ollama is not running on http://localhost:11434." -ForegroundColor Red
    Write-Host "Please start Ollama and run 'ollama pull multilingual-e5-base' manually to prevent vector generation failures." -ForegroundColor Yellow
}

# 6. Database Migrations
$hasEf = Get-Command dotnet-ef -ErrorAction SilentlyContinue
if ($hasEf) {
    Write-Host "Running EF Core database migrations..." -ForegroundColor Yellow
    dotnet ef database update --project "$PSScriptRoot\RagAgent.Infrastructure" --startup-project "$PSScriptRoot\RagAgent.Api"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "WARNING: EF Core migration command failed. The API may still run migrations on startup." -ForegroundColor Yellow
    } else {
        Write-Host "[+] Database migrations completed." -ForegroundColor Green
    }
} else {
    Write-Host "dotnet-ef tool not found. The API will apply migrations automatically on startup." -ForegroundColor Yellow
}

# 7. Start Backend API
Write-Host "Launching Backend API in a new terminal window..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'RagAgent Web API running. Press Ctrl+C to terminate.' -ForegroundColor Green; dotnet run --project `"$PSScriptRoot\RagAgent.Api`"" -WorkingDirectory $PSScriptRoot

# 8. Start Frontend UI
$frontendDir = Join-Path $PSScriptRoot "rag-agent-ui"
if (-not (Test-Path (Join-Path $frontendDir "node_modules"))) {
    Write-Host "Frontend dependencies not found. Installing node packages (npm install)..." -ForegroundColor Yellow
    Push-Location $frontendDir
    npm install
    Pop-Location
}

Write-Host "Launching Frontend UI in a new terminal window..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'RagAgent Angular UI running. Press Ctrl+C to terminate.' -ForegroundColor Green; npm start" -WorkingDirectory $frontendDir

# 9. Open browser
Write-Host "Launching default web browser to frontend..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Start-Process "http://localhost:4200"

Write-Host "=============================================" -ForegroundColor Green
Write-Host "RagAgent launch sequence initiated successfully!" -ForegroundColor Green
Write-Host "Please check the opened terminal windows for logs." -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
