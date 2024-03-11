#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pixel.Storage.Service/Pixel.Storage.Service.csproj", "Pixel.Storage.Service/"]
COPY ["Pixel.Data.Adapters/Pixel.Data.Adapters.csproj", "Pixel.Data.Adapters/"]
COPY ["Pixel.Domain.Core/Pixel.Domain.Core.csproj", "Pixel.Domain.Core/"]
COPY ["Pixel.Messages/Pixel.Messages.csproj", "Pixel.Messages/"]
COPY ["Pixel.Shared.Messages/Pixel.Shared.Messages.csproj", "Pixel.Shared.Messages/"]
RUN dotnet restore "./Pixel.Storage.Service/Pixel.Storage.Service.csproj"
COPY . .
WORKDIR "/src/Pixel.Storage.Service"
RUN dotnet build "./Pixel.Storage.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pixel.Storage.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pixel.Storage.Service.dll"]