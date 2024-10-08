FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything
COPY . .
# Restore as distinct layers
RUN dotnet restore "Chess.Api/Chess.Api.csproj"
# Build and publish a release
RUN dotnet publish "Chess.Api/Chess.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Chess.Api.dll"]