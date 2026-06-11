FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProductApi.slnx", "./"]
COPY ["dotnet-tools.json", "./"]
COPY ["src/ProductApi.API/ProductApi.API.csproj", "src/ProductApi.API/"]
COPY ["src/ProductApi.Application/ProductApi.Application.csproj", "src/ProductApi.Application/"]
COPY ["src/ProductApi.Domain/ProductApi.Domain.csproj", "src/ProductApi.Domain/"]
COPY ["src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj", "src/ProductApi.Infrastructure/"]

RUN dotnet restore "src/ProductApi.API/ProductApi.API.csproj"

COPY . .
RUN dotnet publish "src/ProductApi.API/ProductApi.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductApi.API.dll"]
