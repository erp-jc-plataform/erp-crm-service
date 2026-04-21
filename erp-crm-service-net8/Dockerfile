FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ErpCrmService.WebApi/ErpCrmService.WebApi.csproj", "src/ErpCrmService.WebApi/"]
COPY ["src/ErpCrmService.Application/ErpCrmService.Application.csproj", "src/ErpCrmService.Application/"]
COPY ["src/ErpCrmService.Infrastructure/ErpCrmService.Infrastructure.csproj", "src/ErpCrmService.Infrastructure/"]
COPY ["src/ErpCrmService.Domain/ErpCrmService.Domain.csproj", "src/ErpCrmService.Domain/"]
RUN dotnet restore "src/ErpCrmService.WebApi/ErpCrmService.WebApi.csproj"
COPY . .
RUN dotnet build "src/ErpCrmService.WebApi/ErpCrmService.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/ErpCrmService.WebApi/ErpCrmService.WebApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8003
ENTRYPOINT ["dotnet", "ErpCrmService.WebApi.dll"]
