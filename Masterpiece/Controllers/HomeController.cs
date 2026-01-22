using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var param = await _context.ParameterRepository.GetByNameAsync("MinStarEnquete");
            int.TryParse(param.Waarde, out var intParam);

            // Haal de enquêtes op
            var enqueteReservaties = await _context.ReservatieRepository.GetAllWitEnqueteWithStarParamAsync(intParam);

            // Converteer naar viewmodel
            var enqueteViewModels = enqueteReservaties
                .Select(r => _mapper.Map<EnqueteBeheerViewModel>(r))
                .ToList();

            // Stuur naar view
            return View(enqueteViewModels);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
