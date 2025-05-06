FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/business .

ENV ASPNETCORE_URLS="http://0.0.0.0:51187"

ENTRYPOINT ["dotnet", "Business.Host.dll"]