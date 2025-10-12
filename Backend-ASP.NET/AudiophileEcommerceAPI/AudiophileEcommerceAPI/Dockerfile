# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["AudiophileEcommerceAPI/AudiophileEcommerceAPI.csproj", "AudiophileEcommerceAPI/"]
COPY ["Audiophile.Application/Audiophile.Application.csproj", "Audiophile.Application/"]
COPY ["Audiophile.Domain/Audiophile.Domain.csproj", "Audiophile.Domain/"]
COPY ["Audiophile.Infrastructure/Audiophile.Infrastructure.csproj", "Audiophile.Infrastructure/"]

RUN dotnet restore "AudiophileEcommerceAPI/AudiophileEcommerceAPI.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/AudiophileEcommerceAPI"
RUN dotnet build "AudiophileEcommerceAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AudiophileEcommerceAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create non-root user
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "AudiophileEcommerceAPI.dll"]