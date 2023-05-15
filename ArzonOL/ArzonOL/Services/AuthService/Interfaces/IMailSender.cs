namespace ArzonOL.Services.AuthService.Interfaces;
public interface IMailSender
{
    Task Send(string to, string subject, string body);
}
