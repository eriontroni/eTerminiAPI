# syntax=docker/dockerfile:1
#
# eTermini API (aplikacioni publik i qytetarit)
# Build context: rrënja e repo-s eTerminiAPI
#   docker build -t etermini-api .

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopjo vetëm .csproj-at fillimisht që restore-i të ruhet në cache derisa varësitë nuk ndryshojnë.
COPY eTerminiAPI/eTerminiAPI.API.csproj                     eTerminiAPI/
COPY eTerminiApi.Application/eTerminiAPI.Application.csproj eTerminiApi.Application/
COPY eTerminiAPI.Domain/eTerminiAPI.Domain.csproj           eTerminiAPI.Domain/
COPY eTerminiAPI.Infrastructure/eTerminiAPI.Infrastructure.csproj eTerminiAPI.Infrastructure/

RUN dotnet restore eTerminiAPI/eTerminiAPI.API.csproj

# Pastaj kodin e plotë.
COPY eTerminiAPI/                 eTerminiAPI/
COPY eTerminiApi.Application/     eTerminiApi.Application/
COPY eTerminiAPI.Domain/          eTerminiAPI.Domain/
COPY eTerminiAPI.Infrastructure/  eTerminiAPI.Infrastructure/

RUN dotnet publish eTerminiAPI/eTerminiAPI.API.csproj \
      -c Release \
      -o /app/publish \
      --no-restore \
      /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl përdoret nga healthcheck-u i docker compose.
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Serilog shkruan në ./logs — krijoje dhe jepja pronësinë përdoruesit jo-root.
RUN mkdir -p /app/logs && chown -R app:app /app/logs
USER app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "eTerminiAPI.API.dll"]
