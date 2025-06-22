using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Business.AppsettingClass;
using Business.FileManagement.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Business.Common
{
    public class EmailUtils: ITransientDependency
    {
        private SmtpClient _client = new SmtpClient();
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;

        public EmailUtils(IConfiguration configuration, IOptions<EmailSettings> options)
        {
            _configuration = configuration;
            _emailSettings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body, MyFileInfoDto fileInfoDto = null)
        {
            _client = new SmtpClient
            {
                Host = _emailSettings.Host,
                Port = _emailSettings.Port,
                UseDefaultCredentials = _emailSettings.UseDefaultCredentials,
                Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };
        
            using var mailMessage = new MailMessage()
            {
                Subject = subject,
                From = new MailAddress(_configuration["EmailSettings:From"]),
                Body = body,
                To = { to },
            };
        
            if (fileInfoDto != null)
            {
                // 注意：MemoryStream 不應使用 using 包住，
                // 因為 SMTP 是延遲讀取附件，若提前釋放會導致錯誤。
                // MailMessage.Dispose() 會自動處理 MemoryStream 的釋放。
            
                var memoryStream = new MemoryStream(fileInfoDto.FileContent);
                mailMessage.Attachments.Add(
                    new Attachment(memoryStream, fileInfoDto.FileName + ".xlsx", "application/xlsx"));
            }

            await _client.SendMailAsync(mailMessage);
        }
    }
}