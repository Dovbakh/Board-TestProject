#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Board/Host/Board.Host.DbMigrator/Board.Host.DbMigrator.csproj", "src/Board/Host/Board.Host.DbMigrator/"]
RUN dotnet restore "src/Board/Host/Board.Host.DbMigrator/Board.Host.DbMigrator.csproj"
COPY . .
WORKDIR "/src/src/Board/Host/Board.Host.DbMigrator"
RUN dotnet build "Board.Host.DbMigrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Board.Host.DbMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Board.Host.DbMigrator.dll"]