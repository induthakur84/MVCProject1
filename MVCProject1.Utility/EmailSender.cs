using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MVCProject1.Utility
{
    public class EmailSender : IEmailSender
    {
        private EmailSetting _emailSettings { get;}

        public EmailSender(IOptions<EmailSetting> emailSetting) //Constructor
        {
            _emailSettings = emailSetting.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Excute(email,subject, htmlMessage).Wait();
            return Task.FromResult(0);
        }
      
        public async Task Excute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ?_emailSettings.ToEmail:email;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "My Email Name")
                };
                mail.To.Add(toEmail);
                mail.CC.Add(_emailSettings.CcEmail);
                mail.Subject = "Shopping App:" + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                using (SmtpClient smpt = new SmtpClient (_emailSettings.PrimaryDomain,_emailSettings.PrimaryPort))
                {
                    smpt.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smpt.EnableSsl = true;
                    await smpt.SendMailAsync(mail);
            }

            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }

        } 
            
                     
            
            
           
    }
}
