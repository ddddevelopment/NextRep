FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 6000

ENV ASPNETCORE_URLS=http://+:6000

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Services/Workouts/Workouts.Api/Workouts.Api.csproj", "src/Services/Workouts/Workouts.Api/"]
COPY ["src/Services/Workouts/Workouts.Domain/Workouts.Domain.csproj", "src/Services/Workouts/Workouts.Domain/"]
COPY ["src/Services/Workouts/Workouts.Application/Workouts.Application.csproj", "src/Services/Workouts/Workouts.Application/"]
COPY ["src/Services/Workouts/Workouts.DAL.EF/Workouts.DAL.EF.csproj", "src/Services/Workouts/Workouts.DAL.EF/"]
RUN dotnet restore "src/Services/Workouts/Workouts.Api/Workouts.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Workouts/Workouts.Api"
RUN dotnet build "Workouts.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Workouts.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Workouts.Api.dll"]
