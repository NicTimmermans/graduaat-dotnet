using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Resend;
using Restaurant.Data.Repository.Interfaces;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;
using Restaurant.Services.MailService;

[TestFixture]
public class MailSenderTests
{
    private Mock<IResend> _resendMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IMailRepository> _mailRepoMock;
    private Mock<IReservatieRepository> _reservatieRepoMock;
    private Mock<ITijdslotRepository> _tijdslotRepoMock;
    private Mock<IUserRepository> _userRepoMock;

    private MailSender _mailSender;

    [SetUp]
    public void Setup()
    {
        _resendMock = new Mock<IResend>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _mailRepoMock = new Mock<IMailRepository>();
        _reservatieRepoMock = new Mock<IReservatieRepository>();
        _tijdslotRepoMock = new Mock<ITijdslotRepository>();
        _userRepoMock = new Mock<IUserRepository>();

        _unitOfWorkMock.Setup(u => u.MailRepository).Returns(_mailRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ReservatieRepository).Returns(_reservatieRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.TijdslotRepository).Returns(_tijdslotRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepoMock.Object);

        _mailSender = new MailSender(
            _resendMock.Object,
            _unitOfWorkMock.Object,
            userManager: null, // NIET gebruikt in deze optie
            customLogger: null // NIET gebruikt in deze optie
        );
    }

    [Test]
    public async Task SendWelcomeMail_ShouldGenerateCorrectEmailContent()
    {
        // Arrange
        var mailId = 1;
        var reservatieId = 10;

        var mail = new Mail
        {
            Onderwerp = "Welkom bij [NAAM]",
            Body = "Hallo [VOORNAAM] [ACHTERNAAM]\\n" +
                   "Datum: [DATUM]\\n" +
                   "Tijd: [TIJD]\\n" +
                   "Aantal: [AANTAL]\\n" +
                   "[NAAM]"
        };

        var reservatie = new Reservatie
        {
            KlantId = "user-1",
            TijdSlotId = 5,
            Datum = new DateTime(2025, 2, 1),
            AantalPersonen = 3
        };

        var user = new CustomUser
        {
            Id = "user-1",
            Voornaam = "Anna",
            Achternaam = "Vermeer",
            Email = "anna@test.be"
        };

        var tijdslot = new Tijdslot
        {
            Id = 5,
            Naam = "18:30"
        };

        _mailRepoMock.Setup(r => r.GetByIdAsync(mailId)).ReturnsAsync(mail);
        _reservatieRepoMock.Setup(r => r.GetByIdAsync(reservatieId)).ReturnsAsync(reservatie);
        _userRepoMock.Setup(r => r.GetByIdAsync(reservatie.KlantId)).ReturnsAsync(user);
        _tijdslotRepoMock.Setup(r => r.GetByIdAsync(reservatie.TijdSlotId)).ReturnsAsync(tijdslot);

        EmailMessage capturedMessage = null;

        //Resend mocken zodat we dit kunnen cancellen en niet mails gaan beginnen sturen vanuit unit testen
        _resendMock.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>((msg, ct) => capturedMessage = msg)
            .ReturnsAsync(new ResendResponse<Guid>(Guid.NewGuid(), null));


        // Act
        await _mailSender.SendWelcomeMail(mailId, reservatieId);

        // Assert 
        Assert.That(capturedMessage, Is.Not.Null);
        Assert.That(capturedMessage.From.Email, Is.EqualTo("info@meetjelle.be"));
        Assert.That(capturedMessage.To.Select(t => t.Email), Does.Contain("anna@test.be"));
        Assert.That(capturedMessage.HtmlBody, Does.Contain("Anna"));
        Assert.That(capturedMessage.HtmlBody, Does.Contain("Vermeer"));
        Assert.That(capturedMessage.HtmlBody, Does.Contain("18:30"));
        Assert.That(capturedMessage.HtmlBody, Does.Contain("3"));
        Assert.That(capturedMessage.HtmlBody, Does.Contain("<br>"));

    }
}
