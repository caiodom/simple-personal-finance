# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80


# Base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


# Copy the project files and restore dependencies
COPY ["SimplePersonalFinance.API/SimplePersonalFinance.API.csproj", "src/SimplePersonalFinance.API/"]
COPY ["SimplePersonalFinance.Application/SimplePersonalFinance.Application.csproj", "src/SimplePersonalFinance.Application/"]
COPY ["SimplePersonalFinance.Infrastructure/SimplePersonalFinance.Infrastructure.csproj", "src/SimplePersonalFinance.Infrastructure/"]
COPY ["SimplePersonalFinance.Core/SimplePersonalFinance.Core.csproj", "src/SimplePersonalFinance.Core/"]
COPY ["SimplePersonalFinance.Shared/SimplePersonalFinance.Shared.csproj", "src/SimplePersonalFinance.Shared/"]
RUN dotnet restore "src/SimplePersonalFinance.API/SimplePersonalFinance.API.csproj"
COPY . .
WORKDIR "/src/SimplePersonalFinance.API"
RUN dotnet build "SimplePersonalFinance.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SimplePersonalFinance.API.csproj" -c Release -o /app/publish /p:UseAppHost=false/app/publish /p:UseAppHost=false

# Final stage: runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimplePersonalFinance.API.dll"]