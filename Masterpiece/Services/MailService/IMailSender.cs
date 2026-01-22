namespace Restaurant.Services.MailService;

public interface IMailSender
{
    public Task SendMail(int mailId, Reservatie rs);
    public Task SendWelcomeMail(int mailId, int reservatieId);
    Task SendEvalMail(int mailId, Reservatie rs);
    public Task SendPasswordResetMail(int mailId, string email, string link, Guid token);
    Task SendBestellingMail(int mailId, Reservatie rs, List<Bestelling> bestellingen);
    Task SendRekeningMail(int mailId, Reservatie rs, List<Bestelling> bestellingen, decimal totaal);
}