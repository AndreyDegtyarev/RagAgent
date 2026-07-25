FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["RagAgent.Api/RagAgent.Api.csproj", "RagAgent.Api/"]
COPY ["RagAgent.Application/RagAgent.Application.csproj", "RagAgent.Application/"]
COPY ["RagAgent.Contracts/RagAgent.Contracts.csproj", "RagAgent.Contracts/"]
COPY ["RagAgent.Domain/RagAgent.Domain.csproj", "RagAgent.Domain/"]
COPY ["RagAgent.Infrastructure/RagAgent.Infrastructure.csproj", "RagAgent.Infrastructure/"]
RUN dotnet restore "RagAgent.Api/RagAgent.Api.csproj"

COPY . .
WORKDIR "/src/RagAgent.Api"
RUN dotnet build "RagAgent.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RagAgent.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RagAgent.Api.dll"]
