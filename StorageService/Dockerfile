FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Common.Secuirty/Common.Secuirty.csproj", "Common.Secuirty/"]
COPY ["StorageService/StorageService.csproj", "StorageService/"]
RUN dotnet restore "StorageService/StorageService.csproj"

COPY . .
WORKDIR "/src/StorageService"
RUN dotnet build "StorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StorageService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/uploads

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "StorageService.dll"]