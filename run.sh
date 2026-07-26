#!/usr/bin/env bash

# Ensure script dir is root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Colors for terminal output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}=============================================${NC}"
echo -e "${CYAN}       RagAgent Application Orchestration     ${NC}"
echo -e "${CYAN}=============================================${NC}"

# 1. Prerequisite check
echo -e "${YELLOW}Checking prerequisites...${NC}"

prereqs=("docker" "dotnet" "npm")
missing=0

for cmd in "${prereqs[@]}"; do
    if ! command -v "$cmd" &> /dev/null; then
        echo -e "${RED}[-] Missing prerequisite: $cmd (must be installed and in PATH)${NC}"
        missing=$((missing + 1))
    else
        echo -e "${GREEN}[+] $cmd is available${NC}"
    fi
done

if [ "$missing" -ne 0 ]; then
    echo -e "${RED}Please install all missing prerequisites and run the script again.${NC}"
    exit 1
fi

# 2. Fix PostgreSQL volume mount bug
INIT_SQL_PATH="$SCRIPT_DIR/devops/postgres/init.sql"
if [ -d "$INIT_SQL_PATH" ]; then
    echo -e "${YELLOW}Detected directory 'init.sql' instead of file. Resolving Windows/Unix volume mount issue...${NC}"
    rm -rf "$INIT_SQL_PATH"
    echo "-- Empty init.sql for postgres initialization" > "$INIT_SQL_PATH"
    echo -e "${GREEN}[+] Replaced init.sql folder with a file.${NC}"
elif [ ! -f "$INIT_SQL_PATH" ]; then
    echo -e "${YELLOW}Creating init.sql file to prevent volume mount issue...${NC}"
    echo "-- Empty init.sql for postgres initialization" > "$INIT_SQL_PATH"
fi

# 3. Spin up Docker Compose
echo -e "${YELLOW}Starting Docker containers (PostgreSQL & RabbitMQ)...${NC}"
docker compose -f devops/docker-compose.yaml up -d
if [ $? -ne 0 ]; then
    echo -e "${RED}Failed to start Docker containers. Exiting.${NC}"
    exit 1
fi

# 4. Wait for Docker containers to be healthy
echo -e "${YELLOW}Waiting for database and message broker to be healthy...${NC}"
timeout=60
elapsed=0
healthy=0

while [ "$elapsed" -lt "$timeout" ]; do
    postgres_status=$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}unhealthy{{end}}' rag-postgres 2>/dev/null)
    rabbitmq_status=$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}unhealthy{{end}}' rag-rabbitmq 2>/dev/null)

    if [ "$postgres_status" = "healthy" ] && [ "$rabbitmq_status" = "healthy" ]; then
        echo -e "${GREEN}[+] Infrastructure services are healthy!${NC}"
        healthy=1
        break
    fi

    echo -e "Waiting for services to boot... ($elapsed/$timeout s)"
    sleep 3
    elapsed=$((elapsed + 3))
done

if [ "$healthy" -ne 1 ]; then
    echo -e "${YELLOW}WARNING: Containers did not report healthy state in time. Proceeding anyway...${NC}"
fi

# 5. Check Ollama and Model
echo -e "${YELLOW}Checking local Ollama status...${NC}"
if curl -s -f http://localhost:11434/api/tags > /dev/null; then
    echo -e "${GREEN}[+] Ollama is running.${NC}"
    ollama_list=$(curl -s http://localhost:11434/api/tags)
    if [[ "$ollama_list" == *"multilingual-e5-base"* ]]; then
        echo -e "${GREEN}[+] Ollama model 'multilingual-e5-base' is already present.${NC}"
    else
        echo -e "${YELLOW}Ollama model 'multilingual-e5-base' not found. Attempting to pull directly...${NC}"
        ollama pull multilingual-e5-base
        
        if ollama list | grep -q "multilingual-e5-base"; then
            echo -e "${GREEN}[+] Model pulled successfully.${NC}"
        else
            echo -e "${YELLOW}Direct pull failed. Attempting to pull community model 'qllama/multilingual-e5-base'...${NC}"
            ollama pull qllama/multilingual-e5-base
            
            if ollama list | grep -q "qllama/multilingual-e5-base"; then
                echo -e "${YELLOW}Copying 'qllama/multilingual-e5-base' to 'multilingual-e5-base'...${NC}"
                ollama cp qllama/multilingual-e5-base multilingual-e5-base
                echo -e "${GREEN}[+] Model copied and ready as 'multilingual-e5-base'.${NC}"
            else
                echo -e "${RED}WARNING: Failed to pull community model. Ollama embeddings may fail.${NC}"
            fi
        fi
    fi
else
    echo -e "${RED}WARNING: Ollama is not running on http://localhost:11434.${NC}"
    echo -e "${YELLOW}Please start Ollama and run 'ollama pull multilingual-e5-base' manually to prevent vector generation failures.${NC}"
fi

# 6. Database Migrations
if command -v dotnet-ef &> /dev/null; then
    echo -e "${YELLOW}Running EF Core database migrations...${NC}"
    dotnet ef database update --project RagAgent.Infrastructure --startup-project RagAgent.Api
    if [ $? -ne 0 ]; then
         echo -e "${YELLOW}WARNING: EF Core migration command failed. The API may still run migrations on startup.${NC}"
    else
         echo -e "${GREEN}[+] Database migrations completed.${NC}"
    fi
else
    echo -e "${YELLOW}dotnet-ef tool not found. The API will apply migrations automatically on startup.${NC}"
fi

# Clean up background processes on script exit (Ctrl+C)
cleanup() {
    echo -e "\n${YELLOW}Stopping background application processes...${NC}"
    kill $(jobs -p) 2>/dev/null
    exit 0
}
trap cleanup SIGINT

# 7. Start Backend API
echo -e "${YELLOW}Launching Backend API...${NC}"
dotnet run --project RagAgent.Api &

# 8. Start Frontend UI
FRONTEND_DIR="rag-agent-ui"
if [ ! -d "$FRONTEND_DIR/node_modules" ]; then
    echo -e "${YELLOW}Frontend dependencies not found. Installing node packages (npm install)...${NC}"
    cd "$FRONTEND_DIR" && npm install && cd ..
fi

echo -e "${YELLOW}Launching Frontend UI...${NC}"
cd "$FRONTEND_DIR" && npm start &
cd ..

# 9. Open browser
echo -e "${YELLOW}Launching default web browser to frontend in 5 seconds...${NC}"
sleep 5
if command -v xdg-open &> /dev/null; then
    xdg-open "http://localhost:4200"
elif command -v open &> /dev/null; then
    open "http://localhost:4200"
else
    echo -e "${GREEN}Open http://localhost:4200 in your browser to view the application.${NC}"
fi

echo -e "${GREEN}=============================================${NC}"
echo -e "${GREEN}RagAgent is running! Press Ctrl+C in this terminal to stop.${NC}"
echo -e "${GREEN}=============================================${NC}"

# Keep the shell script running to log background output and wait for cleanup
wait
