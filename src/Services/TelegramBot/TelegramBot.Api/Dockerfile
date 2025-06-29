FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/TelegramBot/TelegramBot.Api/TelegramBot.Api.csproj", "src/Services/TelegramBot/TelegramBot.Api/"]
COPY ["src/Services/TelegramBot/TelegramBot.Application/TelegramBot.Application.csproj", "src/Services/TelegramBot/TelegramBot.Application/"]
COPY ["src/Services/TelegramBot/TelegramBot.Domain/TelegramBot.Domain.csproj", "src/Services/TelegramBot/TelegramBot.Domain/"]
COPY ["src/Services/TelegramBot/TelegramBot.Infrastructure/TelegramBot.Infrastructure.csproj", "src/Services/TelegramBot/TelegramBot.Infrastructure/"]
RUN dotnet restore "./src/Services/TelegramBot/TelegramBot.Api/TelegramBot.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/TelegramBot/TelegramBot.Api"
RUN dotnet build "./TelegramBot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TelegramBot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.Api.dll"] 