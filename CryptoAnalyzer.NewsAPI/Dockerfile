FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CryptoAnalyzer.NewsAPI/CryptoAnalyzer.NewsAPI.csproj", "CryptoAnalyzer.NewsAPI/"]
COPY ["CryptoAnalyzer.News.Core/CryptoAnalyzer.News.Core.csproj", "CryptoAnalyzer.News.Core/"]
COPY ["CryptoAnalyzer.News.Domain/CryptoAnalyzer.News.Domain.csproj", "CryptoAnalyzer.News.Domain/"]
COPY ["CryptoAnalyzer.News.Infrastructure/CryptoAnalyzer.News.Infrastructure.csproj", "CryptoAnalyzer.News.Infrastructure/"]
RUN dotnet restore "CryptoAnalyzer.NewsAPI/CryptoAnalyzer.NewsAPI.csproj"
COPY . .
WORKDIR "/src/CryptoAnalyzer.NewsAPI"
RUN dotnet build "./CryptoAnalyzer.NewsAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CryptoAnalyzer.NewsAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoAnalyzer.NewsAPI.dll"]
