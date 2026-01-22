

namespace Restaurant.Controllers
{
    [Route("Tafel")]
    public class TafelController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public TafelController(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: /Tafel
        [HttpGet("")]
        public IActionResult Index()
        {
            // Altijd eerst naar Tafels/Grondplan
            return RedirectToAction(nameof(TafelGrondplan));
        }

        // ─────────────────────────────────────────────
        // TAFELS (Grondplan / Overzicht)
        // ─────────────────────────────────────────────

        // GET: /Tafel/Tafels/Grondplan
        [HttpGet("Tafels/Grondplan")]
        public async Task<IActionResult> TafelGrondplan()
        {
            var tafels = (await _context.TafelRepository.GetActiveAsync())
                .OrderBy(t => t.TafelNummer)
                .ToList();

            var tiles = _mapper.Map<List<TafelTileViewModel>>(tafels);
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].DisplayOrder = i + 1;
            }

            var vm = new TafelIndexViewModel
            {
                ActiveMain = "Tafels",
                ActiveSub = "Grondplan",
                Grondplan = new TafelGrondplanViewModel { Tafels = tiles },
                Overzicht = new TafelOverzichtViewModel { Tafels = tiles }
            };

            return View("Tafels/TafelIndex", vm);
        }

        // GET: /Tafel/Tafels/Overzicht
        [HttpGet("Tafels/Overzicht")]
        public async Task<IActionResult> TafelOverzicht()
        {
            var tafels = (await _context.TafelRepository.GetActiveAsync())
                .OrderBy(t => t.TafelNummer)
                .ToList();

            var tiles = _mapper.Map<List<TafelTileViewModel>>(tafels);
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].DisplayOrder = i + 1;
            }

            var vm = new TafelIndexViewModel
            {
                ActiveMain = "Tafels",
                ActiveSub = "Overzicht",
                Grondplan = new TafelGrondplanViewModel { Tafels = tiles },
                Overzicht = new TafelOverzichtViewModel { Tafels = tiles }
            };

            return View("Tafels/TafelIndex", vm);
        }

        // ─────────────────────────────────────────────
        // RESERVATIES (Toewijzen / Overzicht)
        // ─────────────────────────────────────────────

        // GET: /Tafel/Reservaties
        [HttpGet("Reservaties")]
        public IActionResult TafelIndexReservatie()
        {
            return RedirectToAction(nameof(TafelToewijzenReservatie));
        }

        // GET: /Tafel/Reservaties/Toewijzen
        [HttpGet("Reservaties/Toewijzen")]
        public IActionResult TafelToewijzenReservatie()
        {
            var vm = new TafelIndexReservatieViewModel
            {
                ActiveMain = "Reservaties",
                ActiveSub = "Toewijzen",
                Toewijzen = new TafelToewijzenReservatieViewModel(),
                Overzicht = new TafelOverzichtReservatieViewModel()
            };

            return View("Reservaties/TafelIndexReservatie", vm);
        }

        // GET: /Tafel/Reservaties/Overzicht
        [HttpGet("Reservaties/Overzicht")]
        public IActionResult TafelOverzichtReservatie()
        {
            var vm = new TafelIndexReservatieViewModel
            {
                ActiveMain = "Reservaties",
                ActiveSub = "Overzicht",
                Toewijzen = new TafelToewijzenReservatieViewModel(),
                Overzicht = new TafelOverzichtReservatieViewModel()
            };

            return View("Reservaties/TafelIndexReservatie", vm);
        }

        // ─────────────────────────────────────────────
        // CRUD-STUBS (optioneel, blijven gewoon bestaan)
        // ─────────────────────────────────────────────

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id) => View();

        [HttpGet("Create")]
        public IActionResult Create() => View();

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id) => View();

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id) => View();

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
