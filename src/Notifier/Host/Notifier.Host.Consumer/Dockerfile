#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Notifier/Host/Notifier.Host.Consumer/Notifier.Host.Consumer.csproj", "src/Notifier/Host/Notifier.Host.Consumer/"]
COPY ["src/Board/Contracts/Board.Contracts/Board.Contracts.csproj", "src/Board/Contracts/Board.Contracts/"]
COPY ["src/Board/Domain/Board.Domain/Board.Domain.csproj", "src/Board/Domain/Board.Domain/"]
COPY ["src/Notifier/Application/Notifier.Application.AppData/Notifier.Application.AppData.csproj", "src/Notifier/Application/Notifier.Application.AppData/"]
COPY ["src/Notifier/Contracts/Notifier.Contracts/Notifier.Contracts.csproj", "src/Notifier/Contracts/Notifier.Contracts/"]
COPY ["src/Notifier/Infrastructure/Notifier.Infrastructure.Registrar/Notifier.Infrastructure.Registrar.csproj", "src/Notifier/Infrastructure/Notifier.Infrastructure.Registrar/"]
RUN dotnet restore "src/Notifier/Host/Notifier.Host.Consumer/Notifier.Host.Consumer.csproj"
COPY . .
WORKDIR "/src/src/Notifier/Host/Notifier.Host.Consumer"
RUN dotnet build "Notifier.Host.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notifier.Host.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifier.Host.Consumer.dll"]