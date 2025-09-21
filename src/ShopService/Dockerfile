# Use official .NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ShopService.csproj", "./"]
RUN dotnet restore "./ShopService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet publish "ShopService.csproj" -c Release -o /app/publish

# Copy the build output to runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShopService.dll"]
