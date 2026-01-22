using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    [Authorize]
    public class OldTafelController : Controller
    {
            private readonly IUnitOfWork _context;
            private readonly IMapper _mapper;

            public OldTafelController(IUnitOfWork context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            // ======================================================
            // INDEX = HOST VOOR TABS (BEHEREN / TOEWIJZEN)
            // ======================================================
            // /Tafel/Index?tab=beheren|toewijzen&filter=...
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            public async Task<IActionResult> Index(string? filter = "active", string? tab = "beheren")
            {
                // 🔧 FIX: als tab == toewijzen, ga direct naar de aparte Toewijzen-pagina
                if (string.Equals(tab, "toewijzen", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(Toewijzen));
                }

                // alle tafels (actief + inactief)
                var tafels = (await _context.TafelRepository.GetAllAsync()).ToList();

                // actieve reservaties, enkel om status / openstaand op kaart te tonen
                var reservaties = await _context.ReservatieRepository.GetAllActiveAsync();

                var tafelsMetReservaties = tafels
                    .Select(t => new TafelReservatiesViewModel
                    {
                        Id = t.Id,
                        TafelNummer = t.TafelNummer ?? $"T{t.Id}",
                        AantalPersonen = t.AantalPersonen,
                        MinAantalPersonen = t.MinAantalPersonen,
                        Actief = t.Actief,
                        Reservaties = reservaties
                            .Where(r => r.Tafellijsten.Any(tl => tl.TafelId == t.Id))
                            .Select(r => new ReservatieItemViewModel
                            {
                                Id = r.Id,
                                Naam = r.CustomUser != null
                                    ? (string.IsNullOrWhiteSpace(r.CustomUser.Voornaam) &&
                                       string.IsNullOrWhiteSpace(r.CustomUser.Achternaam)
                                        ? (r.CustomUser.UserName ?? "Onbekende klant")
                                        : $"{r.CustomUser.Voornaam} {r.CustomUser.Achternaam}".Trim())
                                    : "Onbekend",
                                Datum = r.Datum,
                                IsBetaald = r.Betaald,
                                Totaal = r.Bestellingen
                                    .Where(b => b.StatusId != 5 && b.StatusId != 4)
                                    .Sum(b =>
                                    {
                                        if (b.Product == null) return 0m;

                                        var prijsItem = b.Product.PrijsProducten?
                                            .OrderByDescending(p => p.DatumVanaf)
                                            .FirstOrDefault();

                                        return prijsItem == null ? 0m : prijsItem.Prijs * b.Aantal;
                                    })
                            })
                            .ToList()
                    })
                    .OrderBy(t => t.TafelNummer)
                    .ToList();

                // filter op Actief
                filter ??= "active";
                var normalizedFilter = filter.ToLowerInvariant();

                switch (normalizedFilter)
                {
                    case "all":
                        break;

                    case "inactive":
                        tafelsMetReservaties = tafelsMetReservaties
                            .Where(t => !t.Actief)
                            .ToList();
                        break;

                    default:    // active of onbekend
                        tafelsMetReservaties = tafelsMetReservaties
                            .Where(t => t.Actief)
                            .ToList();
                        normalizedFilter = "active";
                        break;
                }

                var viewModel = new TafelListViewModel
                {
                    Tafels = tafelsMetReservaties,
                    ActiveFilter = normalizedFilter
                };

                ViewBag.ActiveTab = "beheren"; // tab toewijzen gebruiken we niet meer in deze view

                return View(viewModel);
            }


            // ======================================================
            // 2.1.3.15  TAFELS TOEWIJZEN – OVERZICHT
            // ======================================================
            // Use case: overzicht van reservaties, gesorteerd op tafelnummer,
            // met filters op datum en "zonder tafel".
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            public async Task<IActionResult> Toewijzen(DateTime? datum, bool alleenZonderTafel = false)
            {
                var targetDate = datum?.Date ?? DateTime.Today;

                // alle reservaties met user + tijdslot
                var reservaties = await _context.ReservatieRepository.GetAllWithUserAndTijdslotAsync();

                // filter op datum
                reservaties = reservaties
                    .Where(r => r.Datum.HasValue && r.Datum.Value.Date == targetDate)
                    .ToList();

                // sorteren op tafelnummer (reservaties zonder tafel komen laatst)
                reservaties = reservaties
                    .OrderBy(r =>
                        r.Tafellijsten != null && r.Tafellijsten.Any() && r.Tafellijsten.First().Tafel != null
                            ? r.Tafellijsten.First().Tafel!.TafelNummer
                            : "ZZZ")
                    .ToList();

                var vmList = _mapper.Map<List<OldTafelToewijzenReservatieViewModel>>(reservaties);

                if (alleenZonderTafel)
                {
                    vmList = vmList
                        .Where(r => !r.HeeftTafel)
                        .ToList();
                }

                ViewBag.Datum = targetDate.ToString("yyyy-MM-dd");
                ViewBag.AlleenZonderTafel = alleenZonderTafel;

                return View(vmList);
            }

            // ======================================================
            // 2.1.3.15  TAFELS TOEWIJZEN – DETAIL PER RESERVATIE
            // ======================================================

            /// <summary>
            /// GET: reservatiedetails + beschikbare tafels voor datum + tijdslot.
            /// </summary>
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            [HttpGet]
            public async Task<IActionResult> ToewijzenReservatie(int reservatieId)
            {
                // 🔧 Haal reservatie op mét gebruiker + tijdslot
                var reservatiesMetUser = await _context.ReservatieRepository.GetAllWithUserAndTijdslotAsync();
                var reservatie = reservatiesMetUser.FirstOrDefault(r => r.Id == reservatieId);

                if (reservatie == null)
                    return NotFound();

                var vm = _mapper.Map<OldTafelToewijzenReservatieViewModel>(reservatie);

                // alle tafels
                var alleTafels = await _context.TafelRepository.GetAllAsync();

                // alle reservaties op zelfde datum + tijdslot
                var reservatiesZelfdeSlot = await _context.ReservatieRepository
                    .Find(r =>
                        r.Datum == reservatie.Datum &&
                        r.TijdSlotId == reservatie.TijdSlotId) ?? new List<Reservatie>();

                var bezetteTafelIds = new HashSet<int>(
                    reservatiesZelfdeSlot
                        .Where(r => r.Tafellijsten != null)
                        .SelectMany(r => r.Tafellijsten!)
                        .Where(tl => tl.TafelId != 0)
                        .Select(tl => tl.TafelId));

                var beschikbareTafels = alleTafels
                    .Where(t =>
                        t.Actief &&
                        t.AantalPersonen >= reservatie.AantalPersonen &&
                        !bezetteTafelIds.Contains(t.Id))
                    .OrderBy(t => t.TafelNummer)
                    .Select(t => new TafelSelectItemViewModel
                    {
                        Id = t.Id,
                        TafelNummer = t.TafelNummer ?? t.Id.ToString(),
                        AantalPersonen = t.AantalPersonen
                    })
                    .ToList();

                vm.BeschikbareTafels = beschikbareTafels;

                if (!beschikbareTafels.Any())
                {
                    vm.WachttijdMelding =
                        "Alle tafels zijn bezet voor dit tijdslot. " +
                        "Overweeg een later tijdslot of een andere datum als alternatieve optie.";
                }

                return View(vm);
            }

            /// <summary>
            /// POST: geselecteerde tafels koppelen aan reservatie (status = gereserveerd).
            /// </summary>
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ToewijzenReservatie(OldTafelToewijzenReservatieViewModel model)
            {
                if (model.GeselecteerdeTafelIds == null || !model.GeselecteerdeTafelIds.Any())
                {
                    ModelState.AddModelError(string.Empty, "Selecteer minstens één tafel.");
                    return await ToewijzenReservatie(model.ReservatieId);
                }

                var reservatie = await _context.ReservatieRepository.GetByIdAsync(model.ReservatieId);
                if (reservatie == null)
                    return NotFound();

                reservatie.Tafellijsten ??= new List<TafelLijst>();

                foreach (var tafelId in model.GeselecteerdeTafelIds)
                {
                    if (!reservatie.Tafellijsten.Any(tl => tl.TafelId == tafelId))
                    {
                        reservatie.Tafellijsten.Add(new TafelLijst
                        {
                            ReservatieId = reservatie.Id,
                            TafelId = tafelId
                        });
                    }
                }

                // Tafelstatus "gereserveerd" wordt impliciet bepaald door de aanwezigheid
                // van TafelLijst-links binnen dit tijdslot.
                await _context.SaveChangesAsync();

                TempData["ToewijzenSuccess"] = "Tafels succesvol toegewezen.";

                return RedirectToAction(nameof(Toewijzen), new
                {
                    datum = reservatie.Datum?.ToString("yyyy-MM-dd"),
                    alleenZonderTafel = false
                });
            }

            // ======================================================
            // 2.1.3.15  CHECK-IN (TAFEL -> BEZET)
            // ======================================================

            /// <summary>
            /// Check-in: klant is aanwezig, tafel is "bezet" → klant kan bestellen.
            /// </summary>
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CheckIn(int reservatieId)
            {
                var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);
                if (reservatie == null)
                    return NotFound();

                reservatie.IsAanwezig = true;

                await _context.SaveChangesAsync();

                TempData["CheckInSuccess"] = "Klant succesvol ingecheckt. De tafel staat nu op bezet.";

                return RedirectToAction(nameof(Toewijzen), new
                {
                    datum = reservatie.Datum?.ToString("yyyy-MM-dd"),
                    alleenZonderTafel = false
                });
            }

            // ======================================================
            // RESET REKENING (bestaande usecase, kan blijven)
            // ======================================================
            [HttpPost]
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            public async Task<IActionResult> ResetRekening(int reservatieId)
            {
                var reservatie = await _context.ReservatieRepository.GetByIdAsync(reservatieId);
                if (reservatie == null) return NotFound();

                reservatie.Betaald = false;
                _context.SaveChanges();

                return RedirectToAction("Afrekenen");
            }

            // ======================================================
            // CREATE
            // ======================================================
            [HttpGet]
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            public async Task<IActionResult> Create()
            {
                var vm = new TafelCreateViewModel { Actief = true };

                // beschikbare tafel nummers T01–T20, exclusief bestaande
                var existing = (await _context.TafelRepository.GetAllAsync())
                    .Where(t => !string.IsNullOrWhiteSpace(t.TafelNummer))
                    .Select(t => t.TafelNummer!.Trim().ToUpper())
                    .ToHashSet();

                for (int i = 1; i <= 20; i++)
                {
                    var code = $"T{i:00}";
                    if (!existing.Contains(code))
                        vm.BeschikbareNummers.Add(code);
                }

                return View(vm);
            }

            [HttpPost]
            [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(TafelCreateViewModel vm)
            {
                // opnieuw lijst vullen als validation faalt
                var existing = (await _context.TafelRepository.GetAllAsync())
                    .Where(t => !string.IsNullOrWhiteSpace(t.TafelNummer))
                    .Select(t => t.TafelNummer!.Trim().ToUpper())
                    .ToHashSet();

                vm.BeschikbareNummers = new List<string>();
                for (int i = 1; i <= 20; i++)
                {
                    var code = $"T{i:00}";
                    if (!existing.Contains(code))
                        vm.BeschikbareNummers.Add(code);
                }

                if (!ModelState.IsValid)
                    return View(vm);

                if (!vm.BeschikbareNummers.Contains(vm.TafelNummer!.Trim().ToUpper()))
                {
                    ModelState.AddModelError(nameof(vm.TafelNummer),
                        "Dit tafelnummer is niet beschikbaar.");
                    return View(vm);
                }

                var tafel = new Tafel
                {
                    TafelNummer = vm.TafelNummer,
                    AantalPersonen = vm.AantalPersonen,
                    MinAantalPersonen = vm.MinAantalPersonen,
                    Actief = vm.Actief,
                    QrBarcode = vm.QrBarcode
                };

                await _context.TafelRepository.AddAsync(tafel);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { tab = "beheren" });
            }

        // ======================================================
        // EDIT
        // ======================================================
        // EDIT
        // ======================================================
        [HttpGet]
        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        public async Task<IActionResult> Edit(int id)
        {
            var tafel = await _context.TafelRepository.GetByIdAsync(id);
            if (tafel == null) return NotFound();

            var vm = _mapper.Map<TafelEditViewModel>(tafel);

            // ✅ zelfde lijstlogica als Create, maar: eigen nummer mag blijven
            var existing = (await _context.TafelRepository.GetAllAsync())
                .Where(t => t.Id != id) // <- BELANGRIJK: exclude zichzelf
                .Where(t => !string.IsNullOrWhiteSpace(t.TafelNummer))
                .Select(t => t.TafelNummer!.Trim().ToUpper())
                .ToHashSet();

            vm.BeschikbareNummers = new List<string>();
            for (int i = 1; i <= 20; i++)
            {
                var code = $"T{i:00}";
                if (!existing.Contains(code))
                    vm.BeschikbareNummers.Add(code);
            }

            // Zorg dat huidige nummer er zeker in zit (ook als het bv. "T03" is)
            var current = (vm.TafelNummer ?? "").Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(current) && !vm.BeschikbareNummers.Contains(current))
            {
                vm.BeschikbareNummers.Add(current);
                vm.BeschikbareNummers = vm.BeschikbareNummers.OrderBy(x => x).ToList();
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TafelEditViewModel vm)
        {
            // ✅ lijst opnieuw vullen zoals Create (bij validation errors moet dropdown blijven werken)
            var existing = (await _context.TafelRepository.GetAllAsync())
                .Where(t => t.Id != vm.Id) // <- exclude zichzelf
                .Where(t => !string.IsNullOrWhiteSpace(t.TafelNummer))
                .Select(t => t.TafelNummer!.Trim().ToUpper())
                .ToHashSet();

            vm.BeschikbareNummers = new List<string>();
            for (int i = 1; i <= 20; i++)
            {
                var code = $"T{i:00}";
                if (!existing.Contains(code))
                    vm.BeschikbareNummers.Add(code);
            }

            var chosen = (vm.TafelNummer ?? "").Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(chosen) && !vm.BeschikbareNummers.Contains(chosen))
            {
                vm.BeschikbareNummers.Add(chosen);
                vm.BeschikbareNummers = vm.BeschikbareNummers.OrderBy(x => x).ToList();
            }

            if (!ModelState.IsValid) return View(vm);

            // Extra guard: gekozen nummer moet in de lijst zitten
            if (string.IsNullOrWhiteSpace(chosen) || !vm.BeschikbareNummers.Contains(chosen))
            {
                ModelState.AddModelError(nameof(vm.TafelNummer), "Dit tafelnummer is niet beschikbaar.");
                return View(vm);
            }

            var tafel = await _context.TafelRepository.GetByIdAsync(vm.Id);
            if (tafel == null) return NotFound();

            // Zet nummer in uppercase consistent (optioneel maar handig)
            vm.TafelNummer = chosen;

            _mapper.Map(vm, tafel);
            _context.TafelRepository.Update(tafel);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "beheren" });
        }


        // ======================================================
        // DELETE / SOFT DELETE
        // ======================================================
        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? currentFilter)
        {
            var tafel = await _context.TafelRepository.GetByIdAsync(id);
            if (tafel == null) return NotFound();

            var filter = (currentFilter ?? "active").ToLowerInvariant();

            // Hard delete ONLY from "inactive" tab (Opslag)
            var hardDelete = string.Equals(filter, "inactive", StringComparison.OrdinalIgnoreCase);

            if (hardDelete)
            {
                // Block hard delete if ANY TafelLijst exists (past/future/paid/unpaid doesn't matter)
                var hasLinks = await _context.TafelRepository.HasLinkedReservatiesAsync(tafel.Id);
                if (hasLinks)
                {
                    TempData["TafelError"] = "Kan niet definitief verwijderen: deze tafel heeft gekoppelde reservaties.";
                    return RedirectToAction(nameof(Index), new { filter = "inactive", tab = "beheren" });
                }

                _context.TafelRepository.Delete(tafel);
                await _context.SaveChangesAsync();

                TempData["TafelInfo"] = "Tafel definitief verwijderd.";
                return RedirectToAction(nameof(Index), new { filter = "inactive", tab = "beheren" });
            }

            // Active tab => ALWAYS soft delete, no checks
            tafel.Actief = false;
            await _context.SaveChangesAsync();

            TempData["TafelInfo"] = "Tafel verplaatst naar opslag.";
            return RedirectToAction(nameof(Index), new { filter = "active", tab = "beheren" });
        }



        [Authorize(Roles = "Zaalverantwoordelijke,Eigenaar")]
        public async Task<IActionResult> Afrekenen()
        {
            var reservaties = await _context.ReservatieRepository.GetAllActiveAsync();

            var overzicht = reservaties
                .SelectMany(r => r.Tafellijsten.Select(tl => new
                {
                    TafelId = tl.TafelId,
                    TafelNummer = tl.Tafel.TafelNummer,
                    Reservatie = r
                }))
                .GroupBy(x => new { x.TafelId, x.TafelNummer })
                .Select(g => new AfrekenenViewModel
                {
                    TafelId = g.Key.TafelId,
                    TafelNummer = g.Key.TafelNummer,
                    Reservaties = g
                        .Select(x => _mapper.Map<ReservatieItemViewModel>(x.Reservatie))
                        .ToList()
                })
                .OrderBy(x => x.TafelId)
                .ToList();

            return View(overzicht);
        }
    }
}
