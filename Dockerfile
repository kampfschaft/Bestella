# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out ./

# Expose the port for Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Start the app
ENTRYPOINT ["dotnet", "Bestella.dll"]
