
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /

WORKDIR /src
COPY . .

RUN dotnet restore "./Rozklad.V2.csproj"

RUN dotnet publish "Rozklad.V2.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim

EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Rozklad.V2.dll"]
