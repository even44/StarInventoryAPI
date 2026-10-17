FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers (the API project only; the solution also contains the test project)
RUN dotnet restore StarInventoryAPI.csproj
# Build and publish a release
RUN dotnet publish StarInventoryAPI.csproj -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
RUN apt-get update 
RUN apt-get install -y curl
COPY --from=build /App/out .
ENV ConnectionStrings__MariaDbConnection="databaseconnectionstring"
ENV Jwt__Issuer="urltoissuer"
ENV Jwt__ClientId="clientid"
ENTRYPOINT ["dotnet", "StarInventoryAPI.dll"]

