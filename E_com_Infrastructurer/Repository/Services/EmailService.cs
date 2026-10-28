using E_com_Core.Model.User;
using E_com_Core.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace E_com_Infrastructurer.Repository.Services
{
    public class EmailService :IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        public async Task SendEmail(EmailDto emailDTO)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("My Ecom", _configuration["EmailSetting:From"]));
            message.Subject = emailDTO.Subject;
            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Content
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(
                        _configuration["EmailSetting:Smtp"],
                        int.Parse(_configuration["EmailSetting:Port"]),
                        true);



                    await smtp.AuthenticateAsync(
                        _configuration["EmailSetting:Username"],
                        _configuration["EmailSetting:Password"]);

                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                    smtp.Dispose();
                }
            }



        }
    }
}
