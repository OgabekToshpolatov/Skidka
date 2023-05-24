#pragma warning disable
using System;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using ArzonOL.Services.AuthService.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ArzonOL.Services.AuthService
{
    public class MailSender : IMailSender
    {
        private readonly IConfiguration _configuration;

        public MailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(string to, string subject, string body)
        {
            string fromMail = _configuration["MailSender:FromMail"];
            string appPasswordMailRu = _configuration["MailSender:AppPasswordMailRu"];
            string host = _configuration["MailSender:Host"];
            int port = int.Parse(_configuration["MailSender:Port"]);
            bool enableSsl = bool.Parse(_configuration["MailSender:EnableSsl"]);

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(fromMail, appPasswordMailRu);

                using (var message = new MailMessage(new MailAddress(fromMail, "Skidka"), new MailAddress(to)))
                {
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    Console.WriteLine("Sending");

                    client.SendCompleted += (sender, args) =>
                    {
                        Console.WriteLine(sender.GetType().Name);
                        Console.WriteLine(JsonSerializer.Serialize(sender));
                        Console.WriteLine(args.GetType().Name);
                        Console.WriteLine(JsonSerializer.Serialize(args));
                    };

                    await client.SendMailAsync(message);
                    Console.WriteLine(to);
                }
            }
        }
    }
}
