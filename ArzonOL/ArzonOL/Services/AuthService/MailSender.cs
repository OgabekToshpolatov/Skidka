using System.Net;
using System.Net.Mail;
using System.Text.Json;
using ArzonOL.Services.AuthService.Interfaces;

namespace ArzonOL.Services.AuthService;

public class MailSender : IMailSender
{
    private IConfiguration _configuration;

    public MailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task Send(string to, string subject, string body)
    {
        string fromMail = "behzodsafarov73@gmail.com";
        string appPasswordMailRu = "behzod_123bek1";
        SmtpClient client = new SmtpClient
        {
            Host = "smtp.mail.ru",
            Port = 465,
            EnableSsl = true,
            Credentials = new NetworkCredential(fromMail, appPasswordMailRu)
        };

        var message = new MailMessage(new MailAddress(fromMail, "Skidka"), new MailAddress(to));

        var cancellationTokenSource = new CancellationTokenSource();
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml= true;
        Console.WriteLine("Sending");
        client.SendCompleted += (x, y) =>
        {
            Console.WriteLine(x.GetType().Name);
            Console.WriteLine(JsonSerializer.Serialize(x));
            Console.WriteLine(y.GetType().Name);
            Console.WriteLine(JsonSerializer.Serialize(x));
            cancellationTokenSource.Cancel();
        };
        client.SendMailAsync(message, cancellationTokenSource.Token);
        Console.WriteLine(to);
        return Task.CompletedTask;
    }
}