using IdentityAppAPI.DTO.Account;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityAppAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
       
        public async Task<bool> SendEmailAsync(SendEmailDTO emailSend)
        {
            try
            {
                var username = _config["SMTP:Username"];
                var password = _config["SMTP:Password"];
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials=new NetworkCredential(username, password)
                };
                var message = new MailMessage(from: username, to: emailSend.To, subject: emailSend.Subject, body: emailSend.Body);
                message.IsBodyHtml = true;
                await client.SendMailAsync(message);
                return true;
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
