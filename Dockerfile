FROM microsoft/dotnet:2.2-sdk as build
WORKDIR /src
COPY ./ ./authService
WORKDIR /src/authService
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build -c Debug -o /app
# COPY appsettings.Development.json /app
COPY appsettings.Docker.json /app
# COPY appsettings.json /app
# COPY hosting.Development.json /app
COPY hosting.Docker.json /app
# COPY hosting.json /app
#WORKDIR /app
#ENTRYPOINT ["dotnet", "authService.dll"]

# FROM cobreti/linux_netcore_env:1.0 AS final

# FROM microsoft/aspnetcore-build:2.0 AS build
# RUN npm install -g n
# RUN n stable
# WORKDIR /src
# COPY authService.sln ./
# COPY authService ./authService
# WORKDIR /src/authService
# RUN dotnet restore -nowarn:msb3202,nu1503
# RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.2-runtime as final
COPY --from=publish /app /authService
#WORKDIR /authService
#ENTRYPOINT ["dotnet", "authService.dll"]


# FROM cobreti/linux_netcore_env:1.0 AS final
# COPY --from=publish /app /authService
# WORKDIR /authService
# ENTRYPOINT ["dotnet", "authService.dll"]
