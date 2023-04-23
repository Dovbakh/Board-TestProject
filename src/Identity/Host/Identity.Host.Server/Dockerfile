#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Identity/Host/Identity.Host.Server/Identity.Host.Server.csproj", "src/Identity/Host/Identity.Host.Server/"]
COPY ["src/Identity/Application/Identity.Application.AppData/Identity.Application.AppData.csproj", "src/Identity/Application/Identity.Application.AppData/"]
COPY ["src/Identity/Contracts/Identity.Contracts/Identity.Contracts.csproj", "src/Identity/Contracts/Identity.Contracts/"]
COPY ["src/Identity/Clients/Identity.Clients/Identity.Clients.csproj", "src/Identity/Clients/Identity.Clients/"]
COPY ["src/Identity/Infrastructure/Identity.Infrastructure.DataAccess/Identity.Infrastructure.DataAccess.csproj", "src/Identity/Infrastructure/Identity.Infrastructure.DataAccess/"]
COPY ["src/Identity/Domain/Identity.Domain/Identity.Domain.csproj", "src/Identity/Domain/Identity.Domain/"]
COPY ["src/Identity/Infrastructure/Identity.Infrastructure.Migrations/Identity.Infrastructure.Migrations.csproj", "src/Identity/Infrastructure/Identity.Infrastructure.Migrations/"]
COPY ["src/Identity/Infrastructure/Identity.Infrastructure.Registrar/Identity.Infrastructure.Registrar.csproj", "src/Identity/Infrastructure/Identity.Infrastructure.Registrar/"]
COPY ["src/Identity/Infrastructure/Identity.Infrastructure/Identity.Infrastructure.csproj", "src/Identity/Infrastructure/Identity.Infrastructure/"]
RUN dotnet restore "src/Identity/Host/Identity.Host.Server/Identity.Host.Server.csproj"
COPY . .
WORKDIR "/src/src/Identity/Host/Identity.Host.Server"
RUN dotnet build "Identity.Host.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Host.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Host.Server.dll"]