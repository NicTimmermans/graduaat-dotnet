using Hangfire;
using Restaurant.Services.LoggingService;
using System.Net;
using System.Net.Sockets;
using DnsClient;

namespace Restaurant.Services.MailService;

public class MailSender : IMailSender
{
    private readonly IResend _resend;
    private readonly IUnitOfWork _context;
    private readonly UserManager<CustomUser> _userManager;
    private readonly ICustomLogger _customLogger;

    public MailSender(IResend resend, IUnitOfWork context, UserManager<CustomUser> userManager, ICustomLogger customLogger)
    {
        _resend = resend;
        _context = context;
        _userManager = userManager;
        _customLogger = customLogger;
        
        
    }
    public async Task SendMail(int mailId, Reservatie rs)
    {
        var mail = await _context.MailRepository.GetByIdAsync(mailId);   
        var user = await _context.UserRepository.GetByIdAsync(rs.KlantId);
        var tijdslot = await _context.TijdslotRepository.GetByIdAsync(rs.TijdSlotId);
        
        var subject = mail.Onderwerp.Replace("[NAAM]", "Chez Antoine");
        var body = mail.Body
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[ACHTERNAAM]", user.Achternaam)
            .Replace("[DATUM]", rs.Datum.Value.ToShortDateString())
            .Replace("[TIJD]", tijdslot.Naam)
            .Replace("[AANTAL]", rs.AantalPersonen.ToString())
            .Replace("[TAFEL]", "Feature moet nog geimplementeerd worden")
            .Replace("[NAAM]", "Chez Antoine")
            .Replace("\\n", "<br>");
        
        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(user.Email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;
        
        await _resend.EmailSendAsync(message);
          
    }

    public async Task SendWelcomeMail(int mailId, int reservatieId)
    {
        var mail = await _context.MailRepository.GetByIdAsync(mailId);
        var rs = await _context.ReservatieRepository.GetByIdAsync(reservatieId);
        var user = await _context.UserRepository.GetByIdAsync(rs.KlantId);
        var tijdslot = await _context.TijdslotRepository.GetByIdAsync(rs.TijdSlotId);
        
        var subject = mail.Onderwerp.Replace("[NAAM]", "Chez Antoine");
        var body = mail.Body
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[ACHTERNAAM]", user.Achternaam)
            .Replace("[DATUM]", rs.Datum.Value.ToShortDateString())
            .Replace("[TIJD]", tijdslot.Naam)
            .Replace("[AANTAL]", rs.AantalPersonen.ToString())
            .Replace("[TAFEL]", "Feature moet nog geimplementeerd worden")
            .Replace("[NAAM]", "Chez Antoine")
            .Replace("\\n", "<br>");
        
        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(user.Email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;

        await _resend.EmailSendAsync(message);
        
    }

    public async Task SendEvalMail(int mailId, Reservatie rs)
    {
        var mail = await _context.MailRepository.GetByIdAsync(mailId);
        var user = await _context.UserRepository.GetByIdAsync(rs.KlantId);
        var link = $"https://localhost:7044/Enquete?reservatieId={rs.Id}";

        var subject = mail.Onderwerp;
        var body = mail.Body
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[DATUM]", rs.Datum.Value.ToShortDateString())
            .Replace("[LINK]", $"<a href='{link}'>Beoordeling invullen</a>")
            .Replace("\\n", "<br>");


        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(user.Email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;

        await _resend.EmailSendAsync(message);
        

    }
    
    public async Task SendPasswordResetMail(int mailId, string email, string link, Guid token)
    {
        
        var mail = await _context.MailRepository.GetByIdAsync(mailId);
        if (mail == null)
            throw new Exception("Mailtemplate not found");
        
        var user = await _context.UserRepository.GetByEmailAsync(email);
        
        Random rnd = new Random();
        var code = rnd.Next(1000000, 9999999).ToString();
        user.PassWordResetCodeHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(code)
            )
        );
        user.PassWordResetCodeExpiry = DateTime.Now.AddMinutes(15);
        user.ForgotPasswordResetToken = token;
        await _userManager.UpdateAsync(user);
        
        BackgroundJob.Schedule(() => ResetHashCode(user.Email), TimeSpan.FromMinutes(15));
        
        
        var subject = mail.Onderwerp.Replace("[NAAM]", "Chez Antoine");
        var body = mail.Body
            .Replace("\\n", "<br>")
            .Replace("[LINK]", $"<a href='{link}'>Klik hier</a>")
            .Replace("[CODE]", code)
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[NAAM]", "Chez Antoine");
        
        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;
        
        await _resend.EmailSendAsync(message);
        

    }

    public async Task ResetHashCode(string email)
    {
        var user = await _context.UserRepository.GetByEmailAsync(email);
        user.PassWordResetCodeHash = null;
        user.ForgotPasswordResetToken = null;
        await _userManager.UpdateAsync(user);
    }

    public async Task SendBestellingMail(int mailId, Reservatie rs, List<Bestelling> bestellingen)
    {
        var mail = await _context.MailRepository.GetByIdAsync(mailId);
        var user = await _context.UserRepository.GetByIdAsync(rs.KlantId);

        var itemsHtml = "<ul>";

        var grouped = bestellingen
            .GroupBy(b => new { b.Product.Id, b.Product.Naam })
            .Select(g => new
            {
                ProductNaam = g.Key.Naam,
                TotaalAantal = g.Sum(x => x.Aantal)
            });

        foreach (var item in grouped)
        {
            itemsHtml += $"<li>{item.TotaalAantal} x {item.ProductNaam}</li>";
        }

        itemsHtml += "</ul>";

        var subject = mail.Onderwerp.Replace("[DATUM]", rs.Datum.Value.ToShortDateString());
        var body = mail.Body
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[DATUM]", rs.Datum.Value.ToShortDateString())
            .Replace("[ITEMS]", itemsHtml)
            .Replace("\\n", "<br>");


        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(user.Email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;

        await _resend.EmailSendAsync(message);
    }
    public async Task SendRekeningMail(int mailId, Reservatie rs, List<Bestelling> bestellingen, decimal totaal)
    {
        var mail = await _context.MailRepository.GetByIdAsync(mailId);
        var user = await _context.UserRepository.GetByIdAsync(rs.KlantId);

        var itemsHtml = "<ul>";

        var grouped = bestellingen
            .Where(b => b.ReservatieId == rs.Id)
            .GroupBy(b => new { b.Product.Id, b.Product.Naam })
            .Select(g => new
            {
                ProductNaam = g.Key.Naam,
                TotaalAantal = g.Sum(x => x.Aantal)
            });

        foreach (var item in grouped)
        {
            itemsHtml += $"<li>{item.TotaalAantal} x {item.ProductNaam}</li>";
        }

        itemsHtml += "</ul>";


        var subject = mail.Onderwerp.Replace("[DATUM]", rs.Datum.Value.ToShortDateString());
        var body = mail.Body
            .Replace("[VOORNAAM]", user.Voornaam)
            .Replace("[DATUM]", rs.Datum.Value.ToShortDateString())
            .Replace("[ITEMS]", itemsHtml)
            .Replace("[TOTAAL]", $"€ {totaal:F2}")
            .Replace("\\n", "<br>");


        var message = new EmailMessage();
        message.From = "info@meetjelle.be";
        message.To.Add(user.Email);
        message.Subject = subject;
        message.TextBody = null;
        message.HtmlBody = body;

        await _resend.EmailSendAsync(message);
    }
    
}