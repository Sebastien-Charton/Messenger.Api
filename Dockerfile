FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

COPY . .

RUN dotnet restore "src/Web/Web.csproj"

FROM restore AS build

RUN dotnet build "src/Web/Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Web/Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

ARG ASPNETCORE_ENVIRONMENT
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}

WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messenger.Api.Web.dll"]
