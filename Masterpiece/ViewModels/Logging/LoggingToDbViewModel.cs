namespace Restaurant.ViewModels.Logging;

public class LoggingToDbViewModel
{
    public List<Log> Logs { get; set; }
    public string ActiveFilter { get; set; } = "all";
}