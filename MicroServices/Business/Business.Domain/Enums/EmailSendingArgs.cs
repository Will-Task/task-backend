using System.Net.Mail;

namespace Business.Enums;

public class EmailSendingArgs
{
    public string EmailAddress { get; set; }
    
    public string Subject { get; set; }
    
    public string Body { get; set; }
    
    public Attachment? Attachment { get; set; }

}