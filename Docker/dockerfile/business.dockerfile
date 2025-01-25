FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY ./src/business .

# ENTRYPOINT �� CMD �������Á�ָ���������ӕrҪ���е�����
# ENTRYPOINT �������ӕr���У��Ҏ׺����������w��ͨ����춘���һ�����̶������О飬���������đ��ó�ʽ����ʽ��
ENTRYPOINT ["dotnet", "Business.Host.dll"]