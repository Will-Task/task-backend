FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/base .

ENV ASPNETCORE_URLS="http://0.0.0.0:55390"

ENTRYPOINT ["dotnet", "BaseService.Host.dll"]