

using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Restaurant.Hubs;
using Restaurant.Services.LoggingService;
using Restaurant.Services.MailService;

namespace Restaurant.Controllers
{
    [Authorize]
    public class EnqueteController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        private readonly ICustomLogger _logger;


        public EnqueteController(IUnitOfWork context, IMapper mapper, ICustomLogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(int reservatieId)
        {
            var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);
            if (reservatie == null|| !reservatie.Betaald || !reservatie.IsAanwezig) return View("EnqueteVerlopen"); ;

            if (string.IsNullOrEmpty(reservatie.KlantId))
            {
                return Unauthorized();
            }

            var days = 14;
            var para = await _context.ParameterRepository.GetByIdAsync(15);
            if (para != null && int.TryParse(para.Waarde, out var parsedDays))
            {
                days = parsedDays;
            }

            if (!reservatie.Datum.HasValue)
            {
                return BadRequest("Reservatie heeft geen geldige datum.");
            }

            var validUntil = reservatie.Datum.Value.AddDays(days);

            if (DateTime.Now > validUntil)
            {
                TempData["Error"] = "De link voor de enquête is verlopen.";
                return View("EnqueteVerlopen");
            }

            var vm = new EnqueteViewModel
            {
                Id = reservatieId,
                Sterren = reservatie.EvaluatieAantalSterren > 0 ? reservatie.EvaluatieAantalSterren: 1,
                Opmerkingen = reservatie.EvaluatieOpmerkingen
            };

            ViewBag.IsEdit = reservatie.EvaluatieAantalSterren > 0;

            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Enquete(EnqueteViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var reservatie = await _context.ReservatieRepository.GetByIdAsync(model.Id);
            if (reservatie == null) return NotFound();

            reservatie.EvaluatieAantalSterren = model.Sterren;
            reservatie.EvaluatieOpmerkingen = model.Opmerkingen;

            var para = await _context.ParameterRepository.GetByIdAsync(15);
            int.TryParse(para?.Waarde, out var days);
            if (days == 0) days = 14;

            var validUntil = reservatie.Datum.Value.AddDays(days);

            if (reservatie.EvaluatieAantalSterren > 0 && DateTime.Now > validUntil)
            {
                return View("EnqueteVerlopen");
            }

            await _context.SaveChangesAsync();
            await _logger.LogToDb(reservatie.KlantId, "De Enquete is ingevuld", LogStatus.Succes, LogType.Enquete);

            return RedirectToAction("Bedankt");
        }

        public IActionResult Bedankt()
        {
            return View();
            
        }


        //=======================
        //   Enquete beheer
        //=======================
        [Authorize(Roles = "Eigenaar")]
        [HttpGet("EnqueteBeheer")]

        public async Task<IActionResult> EnqueteBeheer()
        {
            var reservaties = await _context.ReservatieRepository.GetAllWitEnqueteAsync();
            var model = _mapper.Map<List<EnqueteBeheerViewModel>>(reservaties);

            
            var param = await _context.ParameterRepository.GetByNameAsync("MinStarEnquete");
            int.TryParse(param?.Waarde, out int minStars);

            ViewBag.MinStarEnquete = minStars;

            return View(model);
        }

        [Authorize(Roles = "Eigenaar")]
        [HttpPost("VerwijderEnquete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderEnquete(int id)
        {

            var reservatie = await _context.ReservatieRepository.GetByIdAsync(id);
            if (reservatie == null)
                return NotFound();


            reservatie.EvaluatieAantalSterren = 0;
            reservatie.EvaluatieOpmerkingen = null;

            await _context.SaveChangesAsync();

            return RedirectToAction("EnqueteBeheer");
        }
        [Authorize(Roles = "Eigenaar")]
        [HttpPost]
        public async Task<IActionResult> UpdateEnqueteParameter(int minStars)
        {
            var param = await _context.ParameterRepository.GetByNameAsync("MinStarEnquete");
            param.Waarde = minStars.ToString();
            

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Parameter gewijzigd!";

            return RedirectToAction("EnqueteBeheer");
        }

    }
}
