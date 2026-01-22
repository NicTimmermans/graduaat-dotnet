using Restaurant.Models;

namespace Restaurant.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public MenuController(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [AllowAnonymous]
        public async Task<ActionResult> Index(int reservatieId = 0, int viewMode = 1)
        {
            ViewBag.ViewMode = viewMode;
            var producten = await _context.MenuRepository.GetProductsAsync();
            var model = new MenuListViewModel
            {
                Producten = _mapper.Map<List<MenuViewModel>>(producten)
            };

            var reservatie = reservatieId != 0
                ? await _context.ReservatieRepository.GetByIdAsync(reservatieId)
                : null;

            ViewBag.ReservatieId = reservatieId;
            ViewBag.MagBestellen =
                User.Identity.IsAuthenticated
                && reservatie != null
                && reservatie.Tafellijsten != null;

            int cartCount = 0;
            int cartCountFull = 0;
            if (reservatie != null)
            {
                var bestellingen = await _context.BestellingRepository.GetByReservatieViewModeAsync(reservatieId, viewMode);
                cartCount = bestellingen.Count(b => b.StatusId == 5);
                cartCountFull = bestellingen.Count();
            }
            ViewBag.CartCount = cartCount;
            ViewBag.CartCountFull = cartCountFull;
    
            return View(model);
        }
    }
}
