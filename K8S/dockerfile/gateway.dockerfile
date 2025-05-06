FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/gateway .

ENV ASPNETCORE_URLS="http://0.0.0.0:62163"

ENTRYPOINT ["dotnet", "WebAppGateway.Host.dll"]