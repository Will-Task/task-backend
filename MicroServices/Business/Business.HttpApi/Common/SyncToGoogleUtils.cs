using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
namespace Business.Common
{
    public class SyncToGoogleUtils
    {
        static readonly string folderName = "googleSync";
        static readonly string fileName = "client_secrets.json";
        static readonly string path = Path.Combine(Environment.CurrentDirectory, folderName, fileName);
        static readonly HttpClient client = new HttpClient();

        public async static Task<string> getToken(string code)
        {

            var content = File.ReadAllText(path);
            /// 解析 JSOn 字串為 JSON Object
            var jsonObject = JsonObject.Parse(content);

            Dictionary<string, string> tuple = new Dictionary<string, string>();
            tuple.Add("client_id", jsonObject["web"]["client_id"].ToString());
            tuple.Add("client_secret", jsonObject["web"]["client_secret"].ToString());
            tuple.Add("code", code);
            tuple.Add("grant_type", "authorization_code");
            tuple.Add("redirect_uri", jsonObject["web"]["redirect_uris"][0].ToString());
            var httpContent = new FormUrlEncodedContent(tuple);

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", httpContent);
            var message = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) != 200)
            {
                throw new Exception($"授權失敗! 無法獲取Token =============== {message}");
            }
            
            jsonObject = JsonObject.Parse(message);

            return jsonObject["access_token"].ToString();
        }
    }
}
