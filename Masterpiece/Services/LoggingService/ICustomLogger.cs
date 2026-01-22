namespace Restaurant.Services.LoggingService;

public interface ICustomLogger
{
    public Task LogToDb(string id, string msg, LogStatus status, LogType logType);
}