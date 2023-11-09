using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Inveon.MailSender
{
    public static class MailSender
    {
        public static void Send(string sendTo, string subject, string body)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("testinveontest@outlook.com");
            mailMessage.To.Add(sendTo);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.Host = "smtp-mail.outlook.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("testinveontest@outlook.com", "Inveon.test");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Debug.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
    }
}