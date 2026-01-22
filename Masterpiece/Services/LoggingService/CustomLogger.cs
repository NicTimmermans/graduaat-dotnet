namespace Restaurant.Services.LoggingService;

public class CustomLogger : ICustomLogger
{
    private readonly IUnitOfWork _context;

    public CustomLogger(IUnitOfWork context)
    {
        _context = context;
    }
    
    
    public async Task LogToDb(string id, string msg, LogStatus status, LogType logType)
    {
        var user = await _context.UserRepository.GetByIdAsync(id);

        Log log = new Log
        {
            UserName = user.UserName,
            Message = msg,
            Date = DateTime.Now,
            LogStatus = status.ToString(),
            LogType = logType.ToString()
        };

        await _context.LogRepository.AddAsync(log);
        await _context.SaveChangesAsync();
        
        
    }
}