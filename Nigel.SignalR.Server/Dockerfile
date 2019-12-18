FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Nigel.SignalR.Server/Nigel.SignalR.Server.csproj", "Nigel.SignalR.Server/"]
RUN dotnet restore "Nigel.SignalR.Server/Nigel.SignalR.Server.csproj"
COPY . .
WORKDIR "/src/Nigel.SignalR.Server"
RUN dotnet build "Nigel.SignalR.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nigel.SignalR.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nigel.SignalR.Server.dll"]