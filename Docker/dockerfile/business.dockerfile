FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/business .

# ENTRYPOINT 和 CMD 都可以用來指定容器啟動時要執行的命令
# ENTRYPOINT 容器啟動時執行，且幾乎不會被覆蓋。通常用於構建一個「固定」的行為，比如執行你的應用程式主程式。
ENTRYPOINT ["dotnet", "Business.Host.dll"]