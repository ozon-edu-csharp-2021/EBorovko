FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Ozon.MerchandiseService.Api/Ozon.MerchandiseService.Api.csproj", "Ozon.MerchandiseService.Api/"]
RUN dotnet restore "src/Ozon.MerchandiseService.Api/Ozon.MerchandiseService.Api.csproj"
COPY . .
WORKDIR "/src/Ozon.MerchandiseService.Api"
RUN dotnet build "Ozon.MerchandiseService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozon.MerchandiseService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.MerchandiseService.Api.dll"]