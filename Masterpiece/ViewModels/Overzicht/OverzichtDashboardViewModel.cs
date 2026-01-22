using System;
using System.Collections.Generic;

namespace Restaurant.ViewModels.Overzicht
{
    public enum OverzichtType
    {
        Reservaties = 1,
        Omzet = 2,
        Feedback = 3
    }

    public enum OverzichtPeriode
    {
        Dag = 1,
        Week = 2,
        Maand = 3
    }

    public class OverzichtFilterViewModel
    {
        public OverzichtType Type { get; set; } = OverzichtType.Reservaties;
        public OverzichtPeriode Periode { get; set; } = OverzichtPeriode.Dag;

        // Startdatum = gekozen dag (voor week/maand wordt er range van gemaakt)
        public DateTime StartDatum { get; set; } = DateTime.Today;

        // alleen handig voor UI
        public DateTime Van { get; set; }
        public DateTime Tot { get; set; }

        public string? InfoMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class OverzichtDashboardViewModel
    {
        public OverzichtFilterViewModel Filter { get; set; } = new();

        public ReservatiesRapportViewModel Reservaties { get; set; } = new();
        public OmzetRapportViewModel Omzet { get; set; } = new();
        public FeedbackRapportViewModel Feedback { get; set; } = new();
    }

    public class ReservatiesRapportViewModel
    {
        public int TotaalReservaties { get; set; }
        public int TotaalPersonen { get; set; }
        public List<ReservatieRapportRow> Rows { get; set; } = new();
    }

    public class ReservatieRapportRow
    {
        public DateTime Datum { get; set; }
        public string TijdslotNaam { get; set; } = "";
        public string KlantNaam { get; set; } = "";
        public int AantalPersonen { get; set; }
        public string Tafels { get; set; } = "";
        public bool IsAanwezig { get; set; }
        public bool Betaald { get; set; }
    }

    public class OmzetRapportViewModel
    {
        public decimal TotaalOmzet { get; set; }
        public int AantalBestellingen { get; set; }
        public List<OmzetRapportRow> Rows { get; set; } = new();
    }

    public class OmzetRapportRow
    {
        public DateTime Datum { get; set; }
        public int ReservatieId { get; set; }
        public string Tafels { get; set; } = "";
        public decimal Bedrag { get; set; }
    }

    public class FeedbackRapportViewModel
    {
        public int AantalFeedbacks { get; set; }
        public double GemiddeldeScore { get; set; }
        public List<FeedbackRapportRow> Rows { get; set; } = new();
    }

    public class FeedbackRapportRow
    {
        public DateTime Datum { get; set; }
        public string KlantNaam { get; set; } = "";
        public int Sterren { get; set; }
        public string? Opmerking { get; set; }
    }
}
