using System; 
using System.Net; 
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bai_Cuoi_Ky.Services 
{
    public class EmailSender 
    { 
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage) 
        { 
            var fromEmail = "datpham14563@gmail.com"; 
            var appPassword = "qvfb jbcn rzwq xzyk"; 

            var message = new MailMessage(fromEmail, toEmail, subject, htmlMessage) 
            {
                IsBodyHtml = true 
            }; 

            using var client = new SmtpClient("smtp.gmail.com", 587) 
            { 
                Credentials = new NetworkCredential(fromEmail, appPassword), 
                EnableSsl = true 
            };

            await client.SendMailAsync(message); 
        }
    } 
} 