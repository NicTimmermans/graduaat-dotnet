using Moq;
using Restaurant.Services.LoggingService;
using System.Threading.Tasks;
using Restaurant.Data.Repository.Interfaces;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;

public class CustomLoggerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogRepository> _logRepositoryMock;

    private readonly CustomLogger _customLogger;

    public CustomLoggerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();

        _unitOfWorkMock.Setup(u => u.UserRepository)
            .Returns(_userRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.LogRepository)
            .Returns(_logRepositoryMock.Object);

        _customLogger = new CustomLogger(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task LogToDb_ShouldAddLogAndSaveChanges()
    {
        // Arrange
        var userId = "123";
        var message = "Test log message";
        var status = LogStatus.Succes;
        var logType = LogType.Bestelling;

        var user = new CustomUser()
        {
            Id = userId,
            UserName = "TestUser"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        await _customLogger.LogToDb(userId, message, status, logType);

        // Assert
        _logRepositoryMock.Verify(repo => repo.AddAsync(
                It.Is<Log>(l =>
                    l.UserName == "TestUser" &&
                    l.Message == message &&
                    l.LogStatus == status.ToString() &&
                    l.LogType == logType.ToString()
                )),
            Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}