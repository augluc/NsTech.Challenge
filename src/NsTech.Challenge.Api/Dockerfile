FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia de csproj individual para otimizar o cache do Docker
COPY ["src/NsTech.Challenge.Api/NsTech.Challenge.Api.csproj", "src/NsTech.Challenge.Api/"]
COPY ["src/NsTech.Challenge.Application/NsTech.Challenge.Application.csproj", "src/NsTech.Challenge.Application/"]
COPY ["src/NsTech.Challenge.Domain/NsTech.Challenge.Domain.csproj", "src/NsTech.Challenge.Domain/"]
COPY ["src/NsTech.Challenge.Infrastructure/NsTech.Challenge.Infrastructure.csproj", "src/NsTech.Challenge.Infrastructure/"]

RUN dotnet restore "src/NsTech.Challenge.Api/NsTech.Challenge.Api.csproj"

COPY . .
WORKDIR "/src/src/NsTech.Challenge.Api"
RUN dotnet build "NsTech.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NsTech.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NsTech.Challenge.Api.dll"]