namespace Restaurant.Models;

public class Log
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public string LogStatus { get; set; }
    public string LogType { get; set; }
}

public enum LogStatus
{
    Succes,
    Warning,
    Error
}

public enum LogType
{
    Reservatie,
    Bestelling,
    Afrekenen,
    Enquete,
    Mail
}