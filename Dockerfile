# Multi-stage Dockerfile for .NET 8 Web API (TrackYourTasksAPI)
# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file(s) and restore dependencies
# If your .csproj has a different name or lives in a subfolder, update the path below.
COPY ["TrackYourTasksAPI.csproj", "./"]
RUN dotnet restore "TrackYourTasksAPI.csproj"

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish "TrackYourTasksAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Optional: enable globalization if needed by your app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:$PORT

COPY --from=build /app/publish .

# Expose a fallback port (Render will supply a PORT env var at runtime)
EXPOSE 10000

ENTRYPOINT ["dotnet", "TrackYourTasksAPI.dll"]