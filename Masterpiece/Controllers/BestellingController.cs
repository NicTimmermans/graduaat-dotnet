using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Restaurant.Hubs;
using Restaurant.Models;
using Restaurant.Services;
using Restaurant.Services.LoggingService;
using Restaurant.Services.MailService;

namespace Restaurant.Controllers
{
    [Authorize]
    public class BestellingController : Controller
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;
        private readonly IMailSender _mailSender;
        private readonly ICustomLogger _logger;
        

        private readonly IHubContext<BestellingHub> _hub;

        public BestellingController(IUnitOfWork context, IMapper mapper, IMailSender mailSender, ICustomLogger logger, IHubContext<BestellingHub> hub)
        {
            _context = context;
            _mapper = mapper;
            _mailSender = mailSender;
            _logger = logger;
            _hub = hub;
        }


        // ---------------------------------------------------------
        //  ALGEMENE BESTELLINGEN (KLANT, BESTELLING TOEVOEGEN, AFREKENEN)
        // ---------------------------------------------------------

        [AllowAnonymous]
        public async Task<IActionResult> Index(int reservatieId, int viewMode)
        {
            var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);

            if (reservatie == null)
                return NotFound();

            if (reservatie.Tafellijsten == null)
                return View("Error", "Gelieve eerst in te checken");

            var bestellingen = await _context.BestellingRepository.GetByReservatieViewModeAsync(reservatieId, viewMode);

            ViewBag.ViewMode = viewMode;

            var bestellingenList = _mapper.Map<List<BestellingViewModel>>(bestellingen);
            var bestellingenCombined = new List<BestellingViewModel>();

            foreach (var item in bestellingenList)
            {
                if (viewMode == 1)
                {
                    if (item.StatusId == 5)
                    {
                        var existing = bestellingenCombined.FirstOrDefault(x => x.ProductId == item.ProductId);

                        if (existing == null)
                            bestellingenCombined.Add(item);
                        else
                            existing.Aantal += item.Aantal;
                    }
                }
                else
                {
                    var existing = bestellingenCombined.FirstOrDefault(x => x.ProductId == item.ProductId);
                    if (existing == null)
                        bestellingenCombined.Add(item);
                    else
                        existing.Aantal += item.Aantal;
                }
            }

            var model = new BestellingListViewModel
            {
                ReservatieId = reservatieId,
                Bestellingen = bestellingenCombined,
                Totaal = await _context.BestellingRepository.GetTotalBestellenAsync(reservatieId),
            };

            return View(model);
        }

        // ------------------------- Add -------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add(BestellingCreateViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest();

            var productExists = await _context.BestellingRepository.ProductExistsAsync(vm.ProductId);
            if (!productExists) return BadRequest("Product does not exist.");

            if (vm.ReservatieId == 0)
                return BadRequest("Geen reservatie geselecteerd.");

            var bestelling = _mapper.Map<Bestelling>(vm);

            bestelling.StatusId = 5;

            await _context.BestellingRepository.AddAsync(bestelling);

            return RedirectToAction("Index", "Menu", new { reservatieId = vm.ReservatieId, viewMode = vm.ViewMode });
        }

        // ------------------------- Update -------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Update(BestellingEditViewModel vm)
        {
            await _context.BestellingRepository.UpdateAantalAsync(vm.Id, vm.Aantal);
            var reservatieId = (await _context.BestellingRepository.GetAsync(vm.Id)).ReservatieId;

            return RedirectToAction("Index", new { reservatieId, viewMode = vm.ViewMode });
        }

        // ------------------------- Remove -------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Remove(BestellingRemoveViewModel vm)
        {
            var item = await _context.BestellingRepository.GetAsync(vm.Id);
            if (item == null) return NotFound();

            int reservatieId = item.ReservatieId;
            await _context.BestellingRepository.RemoveAsync(vm.Id);

            return RedirectToAction("Index", new { reservatieId, viewMode = vm.ViewMode });
        }

        // ------------------------- Confirm -------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Confirm(int reservatieId, int viewMode)
        {
            var bestellingen = await _context.BestellingRepository.GetByReservatieAsync(reservatieId, alleenActief: true);
            var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);

            if (bestellingen == null || !bestellingen.Any())
                return BadRequest("Geen bestellingen gevonden.");

            try
            {
                await _context.BestellingRepository.ConfirmAsync(reservatieId);

                try
                {
                    await _mailSender.SendBestellingMail(17, reservatie, bestellingen);
                }
                catch (Exception mailEx)
                {
                    TempData["Error"] =
                    "Netwerkprobleem. De mail werd niet verstuurd.";
                }

                TempData["Success"] = "Bestelling bevestigd.";
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Netwerkprobleem. Uw bestelling werd bewaard en kan later opnieuw bevestigd worden.";

                return RedirectToAction("Index", new { reservatieId, viewMode });
            }

            await _context.BestellingRepository.ConfirmAsync(reservatieId);
            await _logger.LogToDb(reservatie.KlantId, "De bestelling is succesvol ", LogStatus.Succes, LogType.Bestelling);

            return RedirectToAction("Index", new { reservatieId, viewMode });
        }

        // ------------------------- Betalen -------------------------
        [HttpPost]
        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        public async Task<IActionResult> Betalen(int reservatieId, string betaalMethode, decimal? ontvangenBedrag)
        {
            if (string.IsNullOrEmpty(betaalMethode))
                return BadRequest("Selecteer een betaalmethode.");

            var success = await _context.BestellingRepository.BetalenAsync(reservatieId, betaalMethode, ontvangenBedrag);
            var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);

            if (!success)
            {
                TempData["PaymentError"] =
                    betaalMethode == "Cash"
                    ? "Het ontvangen bedrag is niet correct. Controleer het bedrag."
                    : "Betaling mislukt. Probeer opnieuw.";

                return RedirectToAction("Afrekenen", new { reservatieId });
            }

            var bestellingen = await _context.BestellingRepository.GetAllByReservatieAsync(reservatieId);
            var totaal = await _context.BestellingRepository.GetTotalAfrekenenAsync(reservatieId);

            reservatie.Betaald = true;
            try
            {
                await _mailSender.SendEvalMail(4, reservatie);
                await _mailSender.SendRekeningMail(19, reservatie, bestellingen, totaal);
            }
            catch
            {
                TempData["MailWarning"] =
                    "De betaling is geslaagd, maar de bevestigingsmail kon niet verstuurd worden.";
            }

            TempData["Success"] = "Betaling succesvol verwerkt! Bedankt.";
            await _context.SaveChangesAsync();

            await _logger.LogToDb(reservatie.KlantId, "Betaling succesvol verwerkt", LogStatus.Succes, LogType.Afrekenen);
            return RedirectToAction("Afrekenen", "OldTafel");
        }


        // ------------------------- Afrekenen -------------------------
        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        public async Task<IActionResult> Afrekenen(int reservatieId)
        {
            var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);

            if (reservatie == null)
                return NotFound();

            if (reservatie.Tafellijsten == null)
                return View("Error", "Gelieve eerst in te checken");

            var bestellingen = await _context.BestellingRepository.GetByReservatieAsync(reservatieId, alleenActief: false);

            var bestellingenList = _mapper.Map<List<BestellingViewModel>>(bestellingen);
            var bestellingenCombined = new List<BestellingViewModel>();

            foreach (var item in bestellingenList)
            {
                // Alleen items die effectief op de rekening moeten komen
                if (item.StatusId == 3)
                {
                    var existing = bestellingenCombined.FirstOrDefault(x => x.ProductId == item.ProductId);
                    if (existing == null)
                        bestellingenCombined.Add(item);
                    else
                        existing.Aantal += item.Aantal;
                }
            }

            var eersteTafel = reservatie.Tafellijsten.FirstOrDefault();
            string tafelNummer = eersteTafel?.Tafel?.TafelNummer ?? "-";

            var volgendeReservatie = eersteTafel != null
                ? await _context.ReservatieRepository.GetNextReservationForTableAsync(eersteTafel.TafelId, reservatie.Datum!.Value)
                : null;

            var model = new BestellingListViewModel
            {
                ReservatieId = reservatieId,
                Bestellingen = bestellingenCombined,
                Totaal = await _context.BestellingRepository.GetTotalAfrekenenAsync(reservatieId),

                KlantNaam = reservatie.CustomUser != null
                    ? $"{reservatie.CustomUser.Voornaam} {reservatie.CustomUser.Achternaam}".Trim()
                    : "Onbekende klant",

                TafelNummer = tafelNummer,
                ReservatieDatum = reservatie.Datum,
                VolgendeReservatieDatum = volgendeReservatie?.Datum
            };

            return View(model);
        }

        // ---------------------------------------------------------
        //  OBER - OVERZICHT / STATUS / VERWIJDEREN
        // ---------------------------------------------------------
        [Authorize(Roles = "Eigenaar,Ober")]
        [HttpGet]
        public async Task<IActionResult> OberIndex()
        {
            var bestellingen = await _context.BestellingRepository.GetAllForOberAsync();

            var model = _mapper.Map<List<BestellingOverzichtViewModel>>(bestellingen.GroupBy(b => b.ReservatieId)
                                                       .Select(g => g.AsEnumerable())
                                                       .ToList());

            return View(model);

        }

        [Authorize(Roles = "Eigenaar,Ober")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OberBewerken([FromForm] Dictionary<int, BestellingItemViewModel> items)
        {
            if (items == null || !items.Any())
                return BadRequest("Geen items ontvangen.");

            foreach (var kvp in items)
            {
                var item = kvp.Value;
                await _context.BestellingRepository.UpdateStatusAndNoteAsync(item.BestellingId, item.StatusId, item.Opmerking);
            }

            await _context.SaveChangesAsync();

            //stuur SignalR update naar alle koks
            await _hub.Clients.All.SendAsync("ReceiveUpdate");

            return RedirectToAction("OberIndex");
        }


        //zorgt er alleen voor dat deproducten die nog in behandeling zijn verwijderd worden (die nog in de accordeon staan) worden verwijderd 
        [Authorize(Roles = "Eigenaar,Kok")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderGerechtenInAccordeon(List<int> bestellingIds)
        {
            if (bestellingIds == null || !bestellingIds.Any())
                return BadRequest();

            foreach (var id in bestellingIds)
            {
                await _context.BestellingRepository.RemoveAsync(id);
            }

            return RedirectToAction("OberIndex");
        }


        // ---------------------------------------------------------
        //  KOK - OVERZICHT / PRODUCT TOGGLE
        // ---------------------------------------------------------

        [Authorize(Roles = "Eigenaar,Kok")]
        [HttpGet]
        public async Task<IActionResult> KokIndex()
        {
            var bestellingen = await _context.BestellingRepository.GetAllForKokAsync();

            var model = _mapper.Map<List<BestellingOverzichtViewModel>>(bestellingen.GroupBy(b => b.ReservatieId)
                                                        .Select(g => g.AsEnumerable())
                                                        .ToList());

            return View(model);

        }


        [Authorize(Roles = "Eigenaar,Kok")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProductBeschikbaarheid(int productId)
        {
            var product = await _context.ProductRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            product.Actief = !product.Actief;
            await _context.SaveChangesAsync();

            return RedirectToAction("KokIndex");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Eigenaar,Kok")]
        public async Task<IActionResult> KokBewerken([FromForm] Dictionary<int, BestellingItemViewModel> items)
        {
            if (items != null && items.Any())
            {
                foreach (var kvp in items)
                {
                    var item = kvp.Value;
                    await _context.BestellingRepository.UpdateStatusAndNoteAsync(item.BestellingId, item.StatusId, item.Opmerking);
                }

                await _context.SaveChangesAsync();

                // SignalR trigger
                await _hub.Clients.All.SendAsync("ReceiveUpdate");
            }

            return RedirectToAction("KokIndex");
        }


        [Authorize(Roles = "Eigenaar,Ober,Kok")]
        [HttpGet]
        public async Task<IActionResult> OberData()
        {
            var bestellingen = await _context.BestellingRepository.GetAllForOberAsync();

            var model = _mapper.Map<List<BestellingOverzichtViewModel>>(bestellingen
                .GroupBy(b => b.ReservatieId)
                .Select(g => g.AsEnumerable())
                .ToList());

            // Return alleen de partial view als HTML string
            return PartialView("_OberAccordionPartial", model);
        }

        [Authorize(Roles = "Eigenaar,Ober,Kok")]
        [HttpGet]
        public async Task<IActionResult> KokData()
        {
            var bestellingen = await _context.BestellingRepository.GetAllForKokAsync();

            var model = _mapper.Map<List<BestellingOverzichtViewModel>>(bestellingen
                .GroupBy(b => b.ReservatieId)
                .Select(g => g.AsEnumerable())
                .ToList());

            return PartialView("_KokCardPartial", model);
        }

    }
}
