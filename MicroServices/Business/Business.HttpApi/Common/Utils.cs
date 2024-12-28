namespace Business.Common
{
    public class Utils
    {
        /// <summary>
        /// For Localization => 讓他知道要取哪個json文字
        /// </summary>
        /// <param name="lang"></param>
        public static string GetCulture(int lang)
        {
            return lang switch
            {
                2 => "en",
                3 => "zh-Hans",
                _ => "zh-Hant"
            };
        }
    }
}

