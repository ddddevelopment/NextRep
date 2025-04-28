FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8081

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Services/Users/Users.GrpcService/Users.GrpcService.csproj", "src/Services/Users/Users.GrpcService/"]
RUN dotnet restore "src/Services/Users/Users.GrpcService/Users.GrpcService.csproj"
COPY . .
WORKDIR "/src/src/Services/Users/Users.GrpcService"
RUN dotnet build "Users.GrpcService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Users.GrpcService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.GrpcService.dll"]
