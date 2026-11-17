FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and build/package props first for better Docker layer caching
COPY Directory.Build.props Directory.Packages.props TestProject.sln ./
COPY src/TestProject.Domain/TestProject.Domain.csproj src/TestProject.Domain/
COPY src/TestProject.Application/TestProject.Application.csproj src/TestProject.Application/
COPY src/TestProject.Infrastructure/TestProject.Infrastructure.csproj src/TestProject.Infrastructure/
COPY src/TestProject.WebApi/TestProject.WebApi.csproj src/TestProject.WebApi/
COPY tests/TestProject.UnitTests/TestProject.UnitTests.csproj tests/TestProject.UnitTests/

RUN dotnet restore TestProject.sln

# Copy remaining source and publish WebApi
COPY . .
RUN dotnet publish src/TestProject.WebApi/TestProject.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for container healthcheck
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TestProject.WebApi.dll"]
