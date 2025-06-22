using System.Net;

namespace Business.AppsettingClass
{
    public class EmailSettings
    {
        public int Port { get; set; }
    
        public string Host { get; set; }
    
        public string From { get; set; }
    
        public string Password { get; set; }
    
        public bool UseDefaultCredentials { get; set; }
    
        public bool EnableSsl { get; set; }
    }
}