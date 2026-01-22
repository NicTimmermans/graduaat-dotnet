using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Data.UnitOfWork;
using Restaurant.ViewModels.Overzicht;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class OverzichtController : Controller
    {
        private readonly IUnitOfWork _uow;

        public OverzichtController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OverzichtType type = OverzichtType.Reservaties,
                                              OverzichtPeriode periode = OverzichtPeriode.Dag,
                                              DateTime? startDatum = null)
        {
            var vm = new OverzichtDashboardViewModel();
            vm.Filter.Type = type;
            vm.Filter.Periode = periode;
            vm.Filter.StartDatum = (startDatum ?? DateTime.Today).Date;

            (vm.Filter.Van, vm.Filter.Tot) = BuildRange(vm.Filter.StartDatum, vm.Filter.Periode);

            await FillDashboardAsync(vm);

            return View(vm);
        }

        // PDF download
        // Minimalistisch: zelfde data, aparte view die je later via PDF library rendert.
        [HttpGet]
        public async Task<IActionResult> Pdf(OverzichtType type = OverzichtType.Reservaties,
                                            OverzichtPeriode periode = OverzichtPeriode.Dag,
                                            DateTime? startDatum = null)
        {
            var vm = new OverzichtDashboardViewModel();
            vm.Filter.Type = type;
            vm.Filter.Periode = periode;
            vm.Filter.StartDatum = (startDatum ?? DateTime.Today).Date;

            (vm.Filter.Van, vm.Filter.Tot) = BuildRange(vm.Filter.StartDatum, vm.Filter.Periode);

            await FillDashboardAsync(vm);

            // ✅ Voor nu: HTML preview (DoD “download als PDF” -> zie stap onderaan voor echte PDF)
            // Als je al Rotativa/QuestPDF hebt: vervang dit door echte File() output.
            return View("Pdf", vm);
        }

        // Fallback export (als PDF faalt): CSV
        [HttpGet]
        public async Task<IActionResult> Csv(OverzichtType type = OverzichtType.Reservaties,
                                            OverzichtPeriode periode = OverzichtPeriode.Dag,
                                            DateTime? startDatum = null)
        {
            var vm = new OverzichtDashboardViewModel();
            vm.Filter.Type = type;
            vm.Filter.Periode = periode;
            vm.Filter.StartDatum = (startDatum ?? DateTime.Today).Date;
            (vm.Filter.Van, vm.Filter.Tot) = BuildRange(vm.Filter.StartDatum, vm.Filter.Periode);

            await FillDashboardAsync(vm);

            var csv = CsvBuilder(vm);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"overzicht_{type}_{vm.Filter.Van:yyyyMMdd}_{vm.Filter.Tot:yyyyMMdd}.csv");
        }

        private async Task FillDashboardAsync(OverzichtDashboardViewModel vm)
        {
            try
            {
                var van = vm.Filter.Van;
                var tot = vm.Filter.Tot;

                // 1) Reservaties (met user/tijdslot/tafels)
                var reservaties = (await _uow.ReservatieRepository.GetAllWithUserAndTijdslotAsync())
                    .Where(r => r.Datum.HasValue && r.Datum.Value.Date >= van && r.Datum.Value.Date <= tot)
                    .OrderBy(r => r.Datum)
                    .ThenBy(r => r.TijdSlotId)
                    .ToList();

                vm.Reservaties.TotaalReservaties = reservaties.Count;
                vm.Reservaties.TotaalPersonen = reservaties.Sum(r => r.AantalPersonen);

                vm.Reservaties.Rows = reservaties.Select(r => new ReservatieRapportRow
                {
                    Datum = r.Datum!.Value,
                    TijdslotNaam = r.Tijdslot?.Naam ?? "",
                    KlantNaam = r.CustomUser != null
                        ? $"{r.CustomUser.Voornaam} {r.CustomUser.Achternaam}".Trim()
                        : "Onbekend",
                    AantalPersonen = r.AantalPersonen,
                    Tafels = r.Tafellijsten != null
                        ? string.Join(", ", r.Tafellijsten
                            .Where(tl => tl.Tafel != null)
                            .Select(tl => tl.Tafel!.TafelNummer ?? $"T{tl.TafelId}")
                            .Distinct())
                        : "",
                    IsAanwezig = r.IsAanwezig,
                    Betaald = r.Betaald
                }).ToList();

                // 2) Omzet (hergebruik jullie logica: negeer status 4/5, som prijsproducten * aantal)
                // We nemen reservaties uit dezelfde range en gebruiken Bestellingen die al eager loaded zitten in GetAllActiveAsync
                // => als jullie GetAllActiveAsync enkel toekomst pakt, gebruik GetAllWithUserAndTijdslotAsync + Include Bestellingen in repo later.
                var allActive = await _uow.ReservatieRepository.GetAllActiveAsync(); // bij jullie: vandaag -> toekomst
                var omzetRes = allActive
                    .Where(r => r.Datum.HasValue && r.Datum.Value.Date >= van && r.Datum.Value.Date <= tot)
                    .ToList();

                decimal CalcOmzet(Models.Reservatie r)
                {
                    if (r.Bestellingen == null) return 0m;

                    return r.Bestellingen
                        .Where(b => b.StatusId != 5 && b.StatusId != 4)
                        .Sum(b =>
                        {
                            if (b.Product == null) return 0m;
                            var prijsItem = b.Product.PrijsProducten?
                                .OrderByDescending(p => p.DatumVanaf)
                                .FirstOrDefault();
                            return prijsItem == null ? 0m : prijsItem.Prijs * b.Aantal;
                        });
                }

                vm.Omzet.Rows = omzetRes.Select(r => new OmzetRapportRow
                {
                    Datum = r.Datum!.Value,
                    ReservatieId = r.Id,
                    Tafels = r.Tafellijsten != null
                        ? string.Join(", ", r.Tafellijsten
                            .Where(tl => tl.Tafel != null)
                            .Select(tl => tl.Tafel!.TafelNummer ?? $"T{tl.TafelId}")
                            .Distinct())
                        : "",
                    Bedrag = CalcOmzet(r)
                })
                .Where(x => x.Bedrag > 0)
                .OrderBy(x => x.Datum)
                .ThenBy(x => x.ReservatieId)
                .ToList();

                vm.Omzet.TotaalOmzet = vm.Omzet.Rows.Sum(x => x.Bedrag);
                vm.Omzet.AantalBestellingen = vm.Omzet.Rows.Count;

                // 3) Feedback (jullie repo heeft al GetAllWitEnquete... maar die is niet date-ranged)
                // -> we nemen reservaties uit range en filteren evaluatievelden (pas property names aan indien anders)
                var feedbackRes = (await _uow.ReservatieRepository.GetAllWithUserAndTijdslotAsync())
                    .Where(r => r.Datum.HasValue && r.Datum.Value.Date >= van && r.Datum.Value.Date <= tot)
                    .Where(r => r.EvaluatieAantalSterren != 0)
                    .OrderByDescending(r => r.Datum)
                    .ToList();

                vm.Feedback.Rows = feedbackRes.Select(r => new FeedbackRapportRow
                {
                    Datum = r.Datum!.Value,
                    KlantNaam = r.CustomUser != null
                        ? $"{r.CustomUser.Voornaam} {r.CustomUser.Achternaam}".Trim()
                        : "Onbekend",
                    Sterren = r.EvaluatieAantalSterren,
                    Opmerking = r.EvaluatieOpmerkingen
                }).ToList();

                vm.Feedback.AantalFeedbacks = vm.Feedback.Rows.Count;
                vm.Feedback.GemiddeldeScore = vm.Feedback.Rows.Any()
                    ? vm.Feedback.Rows.Average(x => (double)x.Sterren)
                    : 0.0;

                // No data melding
                var anyData = vm.Reservaties.Rows.Any() || vm.Omzet.Rows.Any() || vm.Feedback.Rows.Any();
                if (!anyData)
                    vm.Filter.InfoMessage = "Geen data beschikbaar voor de gekozen periode.";
            }
            catch
            {
                vm.Filter.ErrorMessage = "Rapportgeneratie faalde. Probeer CSV export als alternatief.";
            }
        }

        private static (DateTime van, DateTime tot) BuildRange(DateTime start, OverzichtPeriode periode)
        {
            start = start.Date;

            return periode switch
            {
                OverzichtPeriode.Dag => (start, start),
                OverzichtPeriode.Week => (start.AddDays(-(int)start.DayOfWeek + (int)DayOfWeek.Monday), start.AddDays(-(int)start.DayOfWeek + (int)DayOfWeek.Monday).AddDays(6)),
                OverzichtPeriode.Maand => (new DateTime(start.Year, start.Month, 1), new DateTime(start.Year, start.Month, 1).AddMonths(1).AddDays(-1)),
                _ => (start, start)
            };
        }

        private static string CsvBuilder(OverzichtDashboardViewModel vm)
        {
            // simpele output per type (voldoende voor “alternatief exportformaat”)
            var van = vm.Filter.Van.ToString("yyyy-MM-dd");
            var tot = vm.Filter.Tot.ToString("yyyy-MM-dd");

            if (vm.Filter.Type == OverzichtType.Reservaties)
            {
                var lines = vm.Reservaties.Rows.Select(r =>
                    $"{r.Datum:yyyy-MM-dd HH:mm};{r.TijdslotNaam};{r.KlantNaam};{r.AantalPersonen};{r.Tafels};{r.IsAanwezig};{r.Betaald}");
                return "Datum;Tijdslot;Klant;Aantal;Tafels;Aanwezig;Betaald\n" + string.Join("\n", lines);
            }

            if (vm.Filter.Type == OverzichtType.Omzet)
            {
                var lines = vm.Omzet.Rows.Select(r =>
                    $"{r.Datum:yyyy-MM-dd};{r.ReservatieId};{r.Tafels};{r.Bedrag}");
                return "Datum;ReservatieId;Tafels;Bedrag\n" + string.Join("\n", lines);
            }

            var fb = vm.Feedback.Rows.Select(r =>
                $"{r.Datum:yyyy-MM-dd};{r.KlantNaam};{r.Sterren};{(r.Opmerking ?? "").Replace(";", ",")}");
            return "Datum;Klant;Sterren;Opmerking\n" + string.Join("\n", fb);
        }
    }
}
