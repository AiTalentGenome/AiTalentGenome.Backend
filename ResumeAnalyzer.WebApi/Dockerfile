# Слой сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем файлы проектов для кэширования слоев (dotnet restore)
COPY ["ResumeAnalyzer.sln", "./"]
COPY ["ResumeAnalyzer.WebApi/ResumeAnalyzer.WebApi.csproj", "ResumeAnalyzer.WebApi/"]
COPY ["ResumeAnalyzer.Domain/ResumeAnalyzer.Domain.csproj", "ResumeAnalyzer.Domain/"]
COPY ["ResumeAnalyzer.Infrastructure/ResumeAnalyzer.Infrastructure.csproj", "ResumeAnalyzer.Infrastructure/"]
COPY ["ResumeAnalyzer.Application/ResumeAnalyzer.Application.csproj", "ResumeAnalyzer.Application/"]

RUN dotnet restore

# Копируем всё остальное и собираем
COPY . .
WORKDIR "/src/ResumeAnalyzer.WebApi"
RUN dotnet publish "ResumeAnalyzer.WebApi.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Создаем папку для логов и копируем базу (если она в проекте)
RUN mkdir -p logs
# Если база лежит в корне WebApi, она скопируется автоматически

EXPOSE 80

USER root

RUN mkdir -p /app/db_store && chmod -R 777 /app/db_store

ENTRYPOINT ["dotnet", "ResumeAnalyzer.WebApi.dll"]