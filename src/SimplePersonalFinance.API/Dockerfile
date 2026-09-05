FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SimplePersonalFinance.API/SimplePersonalFinance.API.csproj", "SimplePersonalFinance.API/"]
COPY ["SimplePersonalFinance.Application/SimplePersonalFinance.Application.csproj", "SimplePersonalFinance.Application/"]
COPY ["SimplePersonalFinance.Infrastructure/SimplePersonalFinance.Infrastructure.csproj", "SimplePersonalFinance.Infrastructure/"]
COPY ["SimplePersonalFinance.Core/SimplePersonalFinance.Core.csproj", "SimplePersonalFinance.Core/"]
COPY ["SimplePersonalFinance.Shared/SimplePersonalFinance.Shared.csproj", "SimplePersonalFinance.Shared/"]

RUN dotnet restore "SimplePersonalFinance.API/SimplePersonalFinance.API.csproj"

COPY . .

RUN dotnet publish "SimplePersonalFinance.API/SimplePersonalFinance.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SimplePersonalFinance.API.dll"]
