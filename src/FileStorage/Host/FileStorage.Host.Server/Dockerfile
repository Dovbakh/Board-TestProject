#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FileStorage/Host/FileStorage.Host.Server/FileStorage.Host.Server.csproj", "src/FileStorage/Host/FileStorage.Host.Server/"]
COPY ["src/FileStorage/Application/FileStorage.Application.AppData/FileStorage.Application.AppData.csproj", "src/FileStorage/Application/FileStorage.Application.AppData/"]
COPY ["src/FileStorage/Contracts/FileStorage.Contracts/FileStorage.Contracts.csproj", "src/FileStorage/Contracts/FileStorage.Contracts/"]
COPY ["src/FileStorage/Infrastructure/FileStorage.Infrastructure.Registrar/FileStorage.Infrastructure.Registrar.csproj", "src/FileStorage/Infrastructure/FileStorage.Infrastructure.Registrar/"]
COPY ["src/FileStorage/Infrastructure/FileStorage.Infrastructure.DataAccess/FileStorage.Infrastructure.DataAccess.csproj", "src/FileStorage/Infrastructure/FileStorage.Infrastructure.DataAccess/"]
COPY ["src/FileStorage/Infrastructure/FileStorage.Infrastructure/FileStorage.Infrastructure.csproj", "src/FileStorage/Infrastructure/FileStorage.Infrastructure/"]
RUN dotnet restore "src/FileStorage/Host/FileStorage.Host.Server/FileStorage.Host.Server.csproj"
COPY . .
WORKDIR "/src/src/FileStorage/Host/FileStorage.Host.Server"
RUN dotnet build "FileStorage.Host.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorage.Host.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStorage.Host.Server.dll"]