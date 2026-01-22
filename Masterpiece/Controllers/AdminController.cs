using Microsoft.AspNetCore.Mvc;
using Restaurant.ViewModels.DashBoard;

namespace Restaurant.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDash _dataService;

        public AdminController(IAdminDash dataService)
        {
            _dataService = dataService;
        }
        
        [Authorize(Roles = "Eigenaar,Kok,Ober,Zaalverantwoordelijke")]
        public IActionResult Index()
        {
           var vm = new AdminDashBoardViewModel()
             {
               AdminData = _dataService.GetAdminButtons(),
               OberData = _dataService.GetOberButtons(),
               KokData = _dataService.GetKokButtons(),
             };
           return View(vm);
        }
    }
}
