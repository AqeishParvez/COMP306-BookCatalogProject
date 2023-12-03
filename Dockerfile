# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official ASP.NET Core SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Test
COPY BookCatalogAPI.sln ./
COPY BookInfoLibrary/*.csproj ./BookInfoLibrary/
COPY BookCatalogAPI/*.csproj ./BookCatalogAPI/

COPY . .

RUN dotnet restore
WORKDIR /src/BookInfoLibrary
RUN dotnet build -c Release -o /app

WORKDIR /src/BookCatalogAPI
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BookCatalogAPI.dll"]

#Test
