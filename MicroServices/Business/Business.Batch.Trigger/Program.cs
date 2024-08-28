// See https://aka.ms/new-console-template for more information

var host = string.Empty;
var path = string.Empty;

if (args.Length == 2)
{
    host = args[0];
    path = args[1];
}
else
{
    Console.WriteLine("不支援的參數數量");
    return;
}

using var httpClient = new HttpClient();
string apiUrl = $"{host}/{path}";
Console.WriteLine($"apiUrl => {apiUrl}");

try
{
    // 代表Get請求
    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("執行結果 正常：" + responseBody);
    }
    else
    {
        Console.WriteLine("執行結果 異常：" + response.StatusCode);
    }
}
catch (Exception ex)
{
    Console.WriteLine("錯誤：" + ex.Message);
}