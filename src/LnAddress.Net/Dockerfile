FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LnAddress.Net.csproj", "LnAddress.Net.csproj"]
RUN dotnet restore "LnAddress.Net.csproj" --no-cache

COPY . .

RUN dotnet build "LnAddress.Net.csproj" -c Release --no-restore
RUN dotnet publish "LnAddress.Net.csproj" -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

COPY --from=build --chown=app:app /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "LnAddress.Net.dll"]