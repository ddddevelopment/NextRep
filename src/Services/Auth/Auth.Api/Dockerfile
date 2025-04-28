FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7080

ENV ASPNETCORE_URLS=http://+:7080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Services/Auth/Auth.Api/Auth.Api.csproj", "src/Services/Auth/Auth.Api/"]
RUN dotnet restore "src/Services/Auth/Auth.Api/Auth.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Auth/Auth.Api"
RUN dotnet build "Auth.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Auth.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
