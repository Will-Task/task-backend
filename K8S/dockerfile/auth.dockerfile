FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/auth .

ENV ASPNETCORE_URLS="http://0.0.0.0:53363"

ENTRYPOINT ["dotnet", "AuthServer.dll"]