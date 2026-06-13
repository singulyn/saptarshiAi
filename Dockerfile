# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj"
RUN dotnet publish "src/Apps/SaptariX.Admin.Mvc/SaptariX.Admin.Mvc.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

RUN mkdir -p /app/App_Data/DataProtectionKeys \
    && chown -R app:app /app/App_Data

USER app
EXPOSE 8080

ENTRYPOINT ["dotnet", "SaptariX.Admin.Mvc.dll"]
