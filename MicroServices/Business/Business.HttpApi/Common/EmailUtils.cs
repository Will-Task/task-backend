using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Business.Common;

public class EmailUtils: ITransientDependency
{
    private SmtpClient _client = new SmtpClient();
    private readonly IConfiguration _configuration;

    public EmailUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        _client.Host = _configuration["EmailSettings:Host"];
        _client.Port = Convert.ToInt16(_configuration["EmailSettings:Port"]);
        _client.UseDefaultCredentials = Convert.ToBoolean(_configuration["EmailSettings:UseDefaultCredentials"]);
        _client.Credentials =
            new NetworkCredential(_configuration["EmailSettings:From"], _configuration["EmailSettings:Password"]);
        _client.EnableSsl = Convert.ToBoolean(_configuration["EmailSettings:EnableSsl"]);
        
        var mailMessage = new MailMessage()
        {
            Subject = subject,
            From = new MailAddress(_configuration["EmailSettings:From"]),
            Body = body,
            To = { to },
        };

        await _client.SendMailAsync(mailMessage);
    }
}