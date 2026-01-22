using Microsoft.AspNetCore.Mvc;
using Restaurant.ViewModels.Logging;

namespace Restaurant.Controllers;

public class DbLogginController : Controller
{
    private readonly IUnitOfWork _context;

    public DbLogginController(IUnitOfWork context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(string filter)
    {
        var logs = await _context.LogRepository.GetAllAsync();
        var vm = new LoggingToDbViewModel()
        {
            Logs = logs.ToList(),
            ActiveFilter = string.IsNullOrEmpty(filter) ? "all" : filter.ToLower()
        };
                    
        return View(vm);
    }
    [Authorize(Roles = "Eigenaar")]
    [HttpPost]
    public async Task<IActionResult> LogVerwijderen(int id, string activeFilter)
    {
        var log = await _context.LogRepository.GetByIdAsync(id);
        if (log == null)
            return NotFound($"Er is een probleem opgetreden de Log met dit id: {id} kan niet worden terug gevonden");
        _context.LogRepository.Delete(log);
        await _context.SaveChangesAsync();

        
        return RedirectToAction(nameof(Index), new { filter = activeFilter});
    }
    
    
}