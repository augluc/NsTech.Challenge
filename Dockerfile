# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto para aproveitar o cache das camadas
COPY ["src/NsTech.Challenge.Api/NsTech.Challenge.Api.csproj", "src/NsTech.Challenge.Api/"]
COPY ["src/NsTech.Challenge.Application/NsTech.Challenge.Application.csproj", "src/NsTech.Challenge.Application/"]
COPY ["src/NsTech.Challenge.Domain/NsTech.Challenge.Domain.csproj", "src/NsTech.Challenge.Domain/"]
COPY ["src/NsTech.Challenge.Infrastructure/NsTech.Challenge.Infrastructure.csproj", "src/NsTech.Challenge.Infrastructure/"]

# Restaura dependências
RUN dotnet restore "src/NsTech.Challenge.Api/NsTech.Challenge.Api.csproj"

# Copia todo o código-fonte e compila
COPY . .
WORKDIR "/src/src/NsTech.Challenge.Api"
RUN dotnet build "NsTech.Challenge.Api.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "NsTech.Challenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final / Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "NsTech.Challenge.Api.dll"]