# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (better layer caching)
COPY ["src/DevDock.API/DevDock.API.csproj", "src/DevDock.API/"]
COPY ["src/DevDock.Application/DevDock.Application.csproj", "src/DevDock.Application/"]
COPY ["src/DevDock.Infrastructure/DevDock.Infrastructure.csproj", "src/DevDock.Infrastructure/"]
COPY ["src/DevDock.Domain/DevDock.Domain.csproj", "src/DevDock.Domain/"]

RUN dotnet restore "src/DevDock.API/DevDock.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/DevDock.API"
RUN dotnet build "DevDock.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "DevDock.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime (final small image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevDock.API.dll"]