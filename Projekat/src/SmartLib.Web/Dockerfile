# SmartLib.Web — Multi-stage Dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiranje .csproj fajlova i restore zavisnosti
COPY src/SmartLib.Core/SmartLib.Core.csproj src/SmartLib.Core/
COPY src/SmartLib.Infrastructure/SmartLib.Infrastructure.csproj src/SmartLib.Infrastructure/
COPY src/SmartLib.Web/SmartLib.Web.csproj src/SmartLib.Web/
RUN dotnet restore src/SmartLib.Web/SmartLib.Web.csproj

# Kopiranje kompletnog source koda i build
COPY src/ src/
RUN dotnet publish src/SmartLib.Web/SmartLib.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "SmartLib.Web.dll"]
