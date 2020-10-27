#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/JsonService/JsonService.Api/JsonService.Api.csproj", "src/JsonService/JsonService.Api/"]
RUN dotnet restore "src/JsonService/JsonService.Api/JsonService.Api.csproj"
COPY . .
WORKDIR "/src/src/JsonService/JsonService.Api"
RUN dotnet build "JsonService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JsonService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JsonService.Api.dll"]