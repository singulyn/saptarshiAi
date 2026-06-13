# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY SaptariX.Platform.sln ./
COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet restore src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj
RUN dotnet publish src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

RUN mkdir -p /app/App_Data/DataProtectionKeys \
    && chown -R app:app /app/App_Data

USER app
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SaptariX.Admin.Mvc.dll"]
