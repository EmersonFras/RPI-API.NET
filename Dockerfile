# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src

# Copy only the csproj first to leverage Docker cache.
COPY ["RPI-API.csproj", "./"]
RUN dotnet restore "RPI-API.csproj"

# Copy the remaining source code and publish the app.
COPY . .
RUN dotnet publish "RPI-API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "RPI-API.dll"]
